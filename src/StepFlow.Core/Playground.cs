﻿using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Elements;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public static Intersection.Context IntersectionContext { get; } = new Intersection.Context();

		public PlayerCharacter? PlayerCharacter { get; set; }

		public PlayerCharacter GetPlayerCharacterRequired() => PropertyRequired(PlayerCharacter, nameof(PlayerCharacter));

		public IList<Obstruction> Obstructions { get; } = new List<Obstruction>();

		public IList<Projectile> Projectiles { get; } = new List<Projectile>();

		public IList<Item> Items { get; } = new List<Item>();

		public IList<Enemy> Enemies { get; } = new List<Enemy>();

		public IList<Place> Places { get; } = new List<Place>();

		public IEnumerable<Material> GetMaterials()
			=> (PlayerCharacter is null ? Enumerable.Empty<Material>() : new Material[] { PlayerCharacter })
				.Concat(Obstructions)
				.Concat(Projectiles)
				.Concat(Items)
				.Concat(Enemies)
				.Concat(Places);
	}
}
