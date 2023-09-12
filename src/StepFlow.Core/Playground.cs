using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StepFlow.Core.Components;

namespace StepFlow.Core
{
	public class Playground : Container
	{
		public const string COLLIDED_NAME = nameof(Collided);
		public const string STRENGTH_NAME = "Strength";
		public const string SCHEDULER_NAME = "Scheduler";
		public const string COLLISION_DAMAGE_NAME = "CollisionDamage";

		public IList<Subject> Subjects { get; } = new List<Subject>();

		public IEnumerable<(Subject, Subject)> GetCollision()
		{
			var instance = Subjects.Select(x => x.Components[COLLIDED_NAME]).OfType<Collided>().ToArray();

			for (var iFirst = 0; iFirst < instance.Length; iFirst++)
			{
				for (var iSecond = iFirst + 1; iSecond < instance.Length; iSecond++)
				{
					var first = instance[iFirst];
					var second = instance[iSecond];
					var firstBorder = first.Next ?? first.Current;
					var secondBorder = second.Next ?? second.Current;
					if (firstBorder is { } && secondBorder is { })
					{
						if (firstBorder.IsCollision(secondBorder))
						{
							yield return ((Subject)instance[iFirst].Container, (Subject)instance[iSecond].Container);
						}
					}
				}
			}
		}
	}
}
