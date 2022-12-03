using System;

namespace StepFlow.Layout
{
	public struct CellPosition
	{
		public CellPosition(int column, int row, int columnSpan, int rowSpan)
		{
			Column = column >= 0 ? column : throw new ArgumentOutOfRangeException(nameof(column));
			Row = row >= 0 ? row : throw new ArgumentOutOfRangeException(nameof(row));
			ColumnSpan = columnSpan >= 1 ? columnSpan : throw new ArgumentOutOfRangeException(nameof(columnSpan));
			RowSpan = rowSpan >= 1 ? rowSpan : throw new ArgumentOutOfRangeException(nameof(rowSpan));
		}

		public CellPosition(int column, int row)
			: this(column, row, 1, 1)
		{
		}

		public int Column { get; set; }

		public int Row { get; set; }

		public int ColumnSpan { get; set; }

		public int RowSpan { get; set; }
	}
}
