using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StepFlow.Core.Components;

namespace StepFlow.Core
{
	public class Playground : Container
	{
		public const string COLLIDED_NAME = nameof(Collided);

		public ICollection<Subject> Subjects { get; } = new HashSet<Subject>();

		public IEnumerable<(Collided, Collided)> GetCollision()
		{
			var instance = Subjects.Select(x => x.Components[COLLIDED_NAME]).OfType<Collided>().ToArray();

			for (var iFirst = 0; iFirst < instance.Length; iFirst++)
			{
				for (var iSecond = iFirst + 1; iSecond < instance.Length; iSecond++)
				{
					var first = instance[iFirst];
					var second = instance[iSecond];
					if (first.Border is { } firstBorder && second.Border is { } secondBorder)
					{
						var firstOffset = firstBorder.Clone(null);
						firstOffset.Offset(first.Offset);
						var secondOffset = secondBorder.Clone(null);
						secondOffset.Offset(second.Offset);

						if (firstOffset.IsCollision(secondOffset))
						{
							yield return (instance[iFirst], instance[iSecond]);
						}
					}
				}
			}
		}
	}
}
