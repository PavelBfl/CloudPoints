using StepFlow.Core;

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
					Table[iCol, iRow] = new HexNodeVm(Source[iCol, iRow]);
				}
			}

			TimeAxis = new AxisVm(Source.TimeAxis);
		}

		public int ColsCount => Source.ColsCount;

		public int RowsCount => Source.RowsCount;

		public HexNodeVm this[int col, int row] => Table[col, row];

		private HexNodeVm[,] Table { get; }

		public AxisVm TimeAxis { get; }
	}
}
