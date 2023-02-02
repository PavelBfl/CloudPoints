using System;
using System.Diagnostics.CodeAnalysis;
using StepFlow.CollectionsNodes;

namespace StepFlow.Core
{
	public class World
	{
		private static void Inverse<T>(T[,] array)
		{
			var colsCount = array.GetLength(0);

			for (var iCol = 0; iCol < colsCount; iCol++)
			{
				for (var iRow = 0; iRow < iCol; iRow++)
				{
					(array[iCol, iRow], array[iRow, iCol]) = (array[iRow, iCol], array[iCol, iRow]);
				}
			}
		}

		public World(int colsCount, int rowsCount, HexOrientation orientation, bool offsetOdd)
		{
			if (colsCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(colsCount));
			}

			if (rowsCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(rowsCount));
			}

			Particles = new ParticlesCollection(this);
			Table = new HexNode[colsCount, rowsCount];

			for (var iCol = 0; iCol < colsCount; iCol++)
			{
				for (var iRow = 0; iRow < rowsCount; iRow++)
				{
					var node = new HexNode(this, iCol, iRow);
					Table[iCol, iRow] = node;
					Grid.Add(node);
				}
			}

			for (var iCol = 0; iCol < colsCount; iCol++)
			{
				for (var iRow = 0; iRow < rowsCount; iRow++)
				{
					var current = Table[iCol, iRow];

					HexNode? nearNode;
					if (TryGetNode(iCol, iRow - 1, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetNode(iCol, iRow + 1, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetNode(iCol - 1, iRow, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetNode(iCol + 1, iRow, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					var rowOffset = (iCol % 2 == 1) == offsetOdd ? 1 : -1;
					if (TryGetNode(iCol + 1, iRow + rowOffset, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetNode(iCol - 1, iRow + rowOffset, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}
				}
			}

			if (orientation == HexOrientation.Pointy)
			{
				Inverse(Table);
			}
		}

		public ParticlesCollection Particles { get; }

		public int ColsCount => Table.GetLength(0);

		public int RowsCount => Table.GetLength(1);

		public HexNode this[int col, int row] => Table[col, row];

		public bool TryGetNode(int col, int row, [MaybeNullWhen(false)] out HexNode result)
		{
			if (col < 0 || ColsCount <= col || row < 0 || RowsCount <= row)
			{
				result = default!;
				return false;
			}
			else
			{
				result = Table[col, row];
				return true;
			}
		}

		private HexNode[,] Table { get; }

		public Graph<HexNode, int> Grid { get; } = new Graph<HexNode, int>();
	}
}
