using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Border;
using StepFlow.Core.Elements;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public PlayerCharacter? PlayerCharacter { get; set; }

		public IList<Obstruction> Obstructions { get; } = new List<Obstruction>();

		public IList<Projectile> Projectiles { get; } = new List<Projectile>();

		public IList<Item> Items { get; } = new List<Item>();

		public IList<Enemy> Enemies { get; } = new List<Enemy>();

		public IEnumerable<Material> GetMaterials()
			=> (PlayerCharacter is null ? Enumerable.Empty<Material>() : new Material[] { PlayerCharacter })
				.Concat(Obstructions)
				.Concat(Projectiles)
				.Concat(Items)
				.Concat(Enemies);

		public IEnumerable<(CollisionPair, CollisionPair)> GetCollision()
		{
			var materials = GetMaterials().ToArray();

			for (var iFirst = 0; iFirst < materials.Length; iFirst++)
			{
				for (var iSecond = iFirst + 1; iSecond < materials.Length; iSecond++)
				{
					var first = materials[iFirst];
					var second = materials[iSecond];

					foreach (var firstCollided in first.GetCollideds())
					{
						var firstBorder = firstCollided.Next ?? firstCollided.Current;

						foreach (var secondCollided in second.GetCollideds())
						{
							var secondBorder = secondCollided.Next ?? secondCollided.Current;

							if (firstBorder is { } && secondBorder is { } && firstBorder.IsCollision(secondBorder))
							{
								yield return (
									new CollisionPair(first, firstCollided),
									new CollisionPair(second, secondCollided)
								);
							}
						}
					}
				}
			}
		}
	}
}
