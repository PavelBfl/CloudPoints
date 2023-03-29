﻿using System.Collections.Generic;

namespace StepFlow.GamePlay
{
	public class Piece : Core.Piece, IParticle
	{
		public Piece(float strengthDefault = 0)
		{
			Strength = new Strength(strengthDefault);
		}

		public Strength Strength { get; }

		public IList<Command> Commands { get; } = new List<Command>();

		public double CollisionDamage { get; set; }

		World IParticle.Owner => (World)OwnerRequired;
	}
}
