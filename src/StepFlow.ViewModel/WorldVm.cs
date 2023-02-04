using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using StepFlow.Core;
using StepFlow.Entities;

namespace StepFlow.ViewModel
{
	public class WorldVm : WrapperVm<World>
	{
		public WorldVm(World source) : base(source, true)
		{
			Table = new HexNodeVm[ColsCount, RowsCount];

			for (var iCol = 0; iCol < ColsCount; iCol++)
			{
				for (var iRow = 0; iRow < RowsCount; iRow++)
				{
					Table[iCol, iRow] = new HexNodeVm(this, Source[iCol, iRow]);
				}
			}

			TimeAxis = new AxisVm();
		}

		public int ColsCount => Source.ColsCount;

		public int RowsCount => Source.RowsCount;

		public HexNodeVm this[int col, int row] => Table[col, row];

		private HexNodeVm[,] Table { get; }

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
							Col = hexNode.Col,
							Row = hexNode.Row,
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
	}
}
