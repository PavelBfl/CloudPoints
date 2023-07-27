using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StepFlow.Core
{
	public class Playground : Container
	{
		public ICollection<Points> Tables { get; } = new HashSet<Points>();

		public IEnumerable<(Points, Points)> DeclareCollision()
		{
			var instance = Tables.ToArray();

			for (var iFirst = 0; iFirst < instance.Length; iFirst++)
			{
				for (var iSecond = 0; iSecond < iFirst; iSecond++)
				{
					var firstTable = instance[iFirst];
					var secondTable = instance[iSecond];
					if (firstTable.Course != secondTable.Course)
					{
						var firstBounds = firstTable.Bounds;
						var secondBounds = secondTable.Bounds;
						firstBounds.Offset(firstTable.Course.ToOffset());
						secondBounds.Offset(secondTable.Course.ToOffset());
						if (firstBounds.IntersectsWith(secondBounds) && GetDetailCollision(firstTable, secondTable))
						{
							yield return (firstTable, secondTable);
						}
					}
				}
			}
		}

		private bool GetDetailCollision(Points first, Points second)
		{
			if (first.Course != Course.None && first.Course.Invert() == second.Course)
			{
				foreach (var firstPoint in first)
				{
					var firstNext = firstPoint;
					firstNext.Offset(first.Course.ToOffset());
					foreach (var secondPoint in second)
					{
						var secondNext = secondPoint;
						secondNext.Offset(second.Course.ToOffset());

						if (firstPoint == secondPoint || firstPoint == secondNext || firstNext == secondPoint || firstNext == secondNext)
						{
							return true;
						}
					}
				}
			}
			else
			{
				foreach (var firstPoint in first)
				{
					firstPoint.Offset(first.Course.ToOffset());
					foreach (var secondPoint in second)
					{
						secondPoint.Offset(second.Course.ToOffset());

						if (firstPoint == secondPoint)
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
