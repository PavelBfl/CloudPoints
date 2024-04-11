﻿using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Elements;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public const int CELL_SIZE_DEFAULT = 50;

		public static Intersection.Context IntersectionContext { get; } = new Intersection.Context();

		public PlayerCharacter GetPlayerCharacterRequired() => Items.OfType<PlayerCharacter>().Single();

		public ICollection<Material> Items { get; } = new HashSet<Material>();
	}
}
