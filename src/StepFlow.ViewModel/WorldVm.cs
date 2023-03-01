using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StepFlow.Common;
using StepFlow.Core;
using StepFlow.Entities;

namespace StepFlow.ViewModel
{
	public class WorldVm : WrapperVm<World>
	{
		public WorldVm(IServiceProvider serviceProvider, int colsCount, int rowsCount, HexOrientation orientation, bool offsetOdd)
			: base(serviceProvider, new World(colsCount, rowsCount, orientation, offsetOdd))
		{
			serviceProvider.GetRequiredService<IWorldProvider>().Add(Source, this);

			Particles = new ParticlesCollectionVm(this);

			foreach (var node in Source.Place.Values)
			{
				new NodeVm(serviceProvider, this, node);
			}

			TimeAxis = new AxisVm();
		}

		public ParticlesCollectionVm Particles { get; }

		public IEnumerable<PieceVm> Pieces => Particles.ViewsModels.OfType<PieceVm>();

		public IEnumerable<NodeVm> Nodes => Particles.ViewsModels.OfType<NodeVm>();

		public AxisVm TimeAxis { get; }

		private PieceVm? current = null;

		public PieceVm? Current
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

		public void TakeStep()
		{
			Source.TakeStep();

			foreach (var particle in Particles.Models.Union(Source.Particles))
			{
				var containsVm = Particles.Contains(particle);
				var containsM = Source.Particles.Contains(particle);

				if (containsVm && !containsM)
				{
					Particles[particle].Dispose();
				}
				else if (!containsVm && containsM)
				{
					switch (particle)
					{
						case Piece piece:
							new PieceVm(ServiceProvider, this, piece);
							break;
						case Node node:
							new NodeVm(ServiceProvider, this, node);
							break;
					}
				}
				else if (containsVm && containsM)
				{
					if (Particles[particle] is PieceVm pieceVm)
					{
						pieceVm.TakeStep();
					}
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

			var links = new Dictionary<object, EntityBase>();
			foreach (var particle in Source.Particles)
			{
				switch (particle)
				{
					case Node hexNode:
						var hexNodeEntity = context.HexNodes.Add(new HexNodeEntity()
						{
							Id = context.GetId(),
							Col = hexNode.Position.X,
							Row = hexNode.Position.Y,
							Owner = worldEntity,
						}).Entity;
						links.Add(hexNode, hexNodeEntity);
						break;
					case Piece piece:
						var pieceEntity = context.Pieces.Add(new PieceEntity()
						{
							Id = context.GetId(),
							Owner = worldEntity,
						}).Entity;
						links.Add(piece, pieceEntity);
						break;
					default:
						break;
				}
			}

			foreach (var piece in Source.Particles.OfType<Piece>())
			{
				if (piece.Current is { } current)
				{
					var pieceEntity = (PieceEntity)links[piece];
					var hexNodeEntity = (HexNodeEntity)links[current];

					pieceEntity.Current = hexNodeEntity;
				}
			}

			context.SaveChanges();
		}

		public static World Load(int worldId)
		{
			using var context = new FlowContext();

			var result = new World(0, 0, HexOrientation.Flat, false);
			foreach (var hexNodeEntity in context.HexNodes.Where(x => x.OwnerId == worldId))
			{
				result.Place.Add(new Node(null, new Point(
					x: hexNodeEntity.Col,
					y: hexNodeEntity.Row
				)));
			}

			return result;
		}
	}
}
