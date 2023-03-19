using System.Collections.Generic;

namespace StepFlow.GamePlay
{
	public class Piece : Core.Piece, IParticle
	{
		public Piece(Context owner, float strengthDefault = 0)
		{
			base.Owner = owner.World;
			Owner = owner;
			Strength = new Strength(strengthDefault);
		}

		public new Context Owner { get; }

		public Strength Strength { get; }

		public IList<Command> Commands { get; } = new List<Command>();

		public double CollisionDamage { get; set; }
	}
}
