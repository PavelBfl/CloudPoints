using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public Playground(Context owner) : base(owner)
		{
		}

		public static IEnumerable<(ICollided, ICollided)> GetCollision(IEnumerable<ICollided> collideds)
		{
			var instance = collideds.ToArray();

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
							yield return (instance[iFirst], instance[iSecond]);
						}
					}
				}
			}
		}

		public PlayerCharacter? PlayerCharacter { get; set; }

		public IEnumerable<Subject> GetAllContent()
		{
			var subjects = new List<Subject>();

			if (PlayerCharacter is { })
			{
				subjects.Add(PlayerCharacter);
			}

			var cache = new HashSet<Subject>();
			GetContent(subjects, cache);

			return cache;
		}

		private void GetContent(IEnumerable<Subject> subjects, HashSet<Subject> cache)
		{
			foreach (var subject in subjects)
			{
				if (cache.Add(subject))
				{
					GetContent(subject.GetContent(), cache);
				}
			}
		}

		public override IEnumerable<Subject> GetContent()
		{
			if (PlayerCharacter is { })
			{
				yield return PlayerCharacter;
			}
		}
	}
}
