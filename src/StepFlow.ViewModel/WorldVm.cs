using System.Collections.Generic;
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

			context.Worlds.Add(new WorldEntity()
			{
				Id = context.GetId(),
			});
		}
	}
}
