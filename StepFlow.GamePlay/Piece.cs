using System.Collections.Generic;
using StepFlow.GamePlay.Commands;

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

		public float CollisionDamage { get; set; }

		World IParticle.Owner => (World)OwnerRequired;

		public object? Data { get; set; }
	}
}
