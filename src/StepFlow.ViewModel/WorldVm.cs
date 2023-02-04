using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using StepFlow.Core;
using StepFlow.Entities;

namespace StepFlow.ViewModel
{
	public class WorldVm : WrapperVm<World>
	{
		public WorldVm(World source) : base(source, true)
		{
			foreach (var node in Source.Place.Values)
			{
				nodes.Add(node.Position, new HexNodeVm(this, node));
			}

			TimeAxis = new AxisVm();
		}

		private Dictionary<Point, HexNodeVm> nodes = new Dictionary<Point, HexNodeVm>();

		public IReadOnlyDictionary<Point, HexNodeVm> Nodes => nodes;

		public AxisVm TimeAxis { get; }

		public ICollection<IPieceVm> Pieces { get; } = new HashSet<IPieceVm>();

		private IPieceVm? current = null;

		public IPieceVm? Current
		{
			get => current;
			set
			{
				if (!Equals(Current, value))
				{
					OnPropertyChanging();

					if (Current is { })
					{
						Current.IsMark = false;
					}

					current = value;

					if (Current is { })
					{
						Current.IsMark = true;
					}

					OnPropertyChanged();
				}
			}
		}

		public void Save()
		{
			using var context = new FlowContext();
			context.InitCurrentId();

			var worldEntity = context.Worlds.Add(new WorldEntity()
			{
				Id = context.GetId(),
			}).Entity;

			var links = new Dictionary<(object obj, Type type), EntityBase>();
			foreach (var particle in Source.Particles)
			{
				var particleEntity = context.Particles.Add(new ParticleEntity()
				{
					Id = context.GetId(),
					Owner = worldEntity,
				}).Entity;
				links.Add((particle, typeof(Particle)), particleEntity);

				switch (particle)
				{
					case HexNode hexNode:
						var hexNodeEntity = context.HexNodes.Add(new HexNodeEntity()
						{
							Id = context.GetId(),
							Col = hexNode.Position.X,
							Row = hexNode.Position.Y,
							Particle = particleEntity,
						}).Entity;
						links.Add((hexNode, typeof(HexNode)), hexNodeEntity);
						break;
					case Piece piece:
						var pieceEntity = context.Pieces.Add(new PieceEntity()
						{
							Id = context.GetId(),
							Particle = particleEntity,
						}).Entity;
						links.Add((piece, typeof(Piece)), pieceEntity);
						break;
					default:
						break;
				}
			}

			foreach (var piece in Source.Particles.OfType<Piece>())
			{
				if (piece.Current is { } current)
				{
					var pieceEntity = (PieceEntity)links[(piece, typeof(Piece))];
					var hexNodeEntity = (HexNodeEntity)links[(current, typeof(HexNode))];

					pieceEntity.Current = hexNodeEntity;
				}
			}

			context.SaveChanges();
		}

		public static World Load(int worldId)
		{
			using var context = new FlowContext();

			var result = new World(0, 0, HexOrientation.Flat, false);
			foreach (var hexNodeEntity in context.HexNodes.Where(x => x.Particle.OwnerId == worldId))
			{
				result.Place.Add(new HexNode(null, new Point(
					x: hexNodeEntity.Col,
					y: hexNodeEntity.Row
				)));
			}

			return result;
		}
	}
}
