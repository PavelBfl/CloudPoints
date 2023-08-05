using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StepFlow.Core
{
	public class Playground : Container
	{
		public static IEnumerable<(IBordered, IBordered)> GetCollisions(IEnumerable<IBordered> borders)
		{
			var instance = borders.ToArray();
			for (var iFirst = 0; iFirst < instance.Length; iFirst++)
			{
				for (var iSecond = iFirst + 1; iSecond < instance.Length; iSecond++)
				{
					if (instance[iFirst].IsCollision(instance[iSecond]))
					{
						yield return (instance[iFirst], instance[iSecond]);
					}
				}
			}
		}
	}
}
