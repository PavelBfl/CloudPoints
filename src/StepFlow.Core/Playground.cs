using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StepFlow.Core.Components;

namespace StepFlow.Core
{
	public class Playground : Container
	{
		public const string COLLIDED_NAME = nameof(Collided);

		public IList<Subject> Subjects { get; } = new List<Subject>();

		public IEnumerable<(Collided, Collided)> GetCollision()
		{
			var instance = Subjects.Select(x => x.Components[COLLIDED_NAME]).OfType<Collided>().ToArray();

			for (var iFirst = 0; iFirst < instance.Length; iFirst++)
			{
				for (var iSecond = iFirst + 1; iSecond < instance.Length; iSecond++)
				{
					var first = instance[iFirst];
					var second = instance[iSecond];
					if (first.Next is { } firstBorder && second.Next is { } secondBorder)
					{
						if (firstBorder.IsCollision(secondBorder))
						{
							yield return (instance[iFirst], instance[iSecond]);
						}
					}
				}
			}
		}
	}
}
