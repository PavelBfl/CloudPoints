using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Entities;
using StepFlow.GamePlay;

namespace StepFlow.ViewModel
{
	public class ContextVm : WrapperVm<Context>
	{
		public ContextVm(IServiceProvider serviceProvider, int colsCount, int rowsCount)
			: base(serviceProvider, new Context(colsCount, rowsCount))
		{
			Particles = new ParticlesCollectionVm(this);

			foreach (var node in Source.World.Place.Values)
			{
				new NodeVm(serviceProvider, this, node);
			}

			TimeAxis = new AxisVm(ServiceProvider, Source.AxisTime);
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
			Source.World.TakeStep();

			foreach (var particle in Particles.Models.Union(Source.World.Particles))
			{
				var containsVm = Particles.Contains(particle);
				var containsM = Source.World.Particles.Contains(particle);

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
					Particles[particle].TakeStep();
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
			foreach (var particle in Source.World.Particles)
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

			foreach (var piece in Source.World.Particles.OfType<Piece>())
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

			var result = new World(0, 0);
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
