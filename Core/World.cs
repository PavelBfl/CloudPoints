using CloudPoints;
using TimeLine;

namespace Core
{
	public class World
	{
		public World()
		{
			const int COLS_COUNT = 10;
			const int ROWS_COUNT = 10;

			Table = new HexNode[COLS_COUNT, ROWS_COUNT];

			for (var iCol = 0; iCol < COLS_COUNT; iCol++)
			{
				for (var iRow = 0; iRow < ROWS_COUNT; iRow++)
				{
					var node = new HexNode(this);
					Table[iCol, iRow] = node;
					Grid.Add(node);
				}
			}

			for (var iCol = 0; iCol < COLS_COUNT; iCol++)
			{
				for (var iRow = 0; iRow < ROWS_COUNT; iRow++)
				{
					var current = Table[iCol, iRow];

					HexNode nearNode;
					if (TryGetItem(Table, iCol, iRow - 1, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetItem(Table, iCol, iRow + 1, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetItem(Table, iCol - 1, iRow, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetItem(Table, iCol + 1, iRow, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetItem(Table, iCol + 1, iRow + 1, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}

					if (TryGetItem(Table, iCol - 1, iRow - 1, out nearNode))
					{
						Grid.Add(current, nearNode, 1);
					}
				}
			}
		}

		private static bool TryGetItem(HexNode[,] grid, int col, int row, out HexNode result)
		{
			if (col < 0 && grid.GetLength(0) <= col && row < 0 && grid.GetLength(1) <= row)
			{
				result = default;
				return false;
			}
			else
			{
				result = grid[col, row];
				return true;
			}
		}

		public HexNode[,] Table { get; }

		public Graph<HexNode, int> Grid { get; } = new Graph<HexNode, int>();

		public Axis TimeAxis { get; } = new Axis();
	}
}
