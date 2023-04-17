using System;

namespace StepFlow.Layout
{
	public struct CellPosition : IEquatable<CellPosition>
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

		public readonly bool Equals(CellPosition other)
			=> Column == other.Column && Row == other.Row && ColumnSpan == other.ColumnSpan && RowSpan == other.RowSpan;

		public override readonly bool Equals(object obj) => obj is CellPosition other && Equals(other);

		public override readonly int GetHashCode() => HashCode.Combine(Column, Row, ColumnSpan, RowSpan);

		public static bool operator ==(CellPosition x, CellPosition y) => x.Equals(y);

		public static bool operator !=(CellPosition x, CellPosition y) => !(x == y);
	}
}
