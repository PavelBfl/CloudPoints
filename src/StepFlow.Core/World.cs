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

			Orientation = orientation;
			OffsetOdd = offsetOdd;
			Particles = new ParticlesCollection(this);
			Place = new Place(this);

			for (var iCol = 0; iCol < colsCount; iCol++)
			{
				for (var iRow = 0; iRow < rowsCount; iRow++)
				{
					var position = new System.Drawing.Point(iCol, iRow);
					Place.Add(new HexNode(null, position));
				}
			}
		}

		public HexOrientation Orientation { get; }

		public bool OffsetOdd { get; }

		public ParticlesCollection Particles { get; }

		public Place Place { get; }
	}
}
