using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Border;
using StepFlow.Core.Components;

namespace StepFlow.Core
{
    public class Playground
	{
		private Dictionary<uint, IChild> objects = new Dictionary<uint, IChild>();

		public IReadOnlyDictionary<uint, IChild> Objects => objects;

		public void Register(IChild identity)
		{
			if (identity is null)
			{
				throw new ArgumentNullException(nameof(identity));
			}

			objects.Add(identity.Id, identity);
		}

		public bool Unregister(uint id) => objects.Remove(id);

		private uint currentId = 0;

		public uint GenerateId() => currentId++;

		public IList<Subject> Subjects { get; } = new List<Subject>();

		public IEnumerable<(Collided, Collided)> GetCollision()
		{
			var instance = Subjects.SelectMany(x => x.Components.OfType<Collided>()).ToArray();

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
	}
}
