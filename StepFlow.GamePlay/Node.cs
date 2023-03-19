using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.GamePlay
{
	public class Node : Core.Node, IParticle
	{
		public Node(Context owner, Point position, float strengthDefault = 0)
			: base(position)
		{
			base.Owner = owner.World;
			Owner = owner;
			Strength = new Strength(strengthDefault);
		}

		public Context Owner { get; }

		public Strength Strength { get; }

		public IList<Command> Commands { get; } = new List<Command>();
	}
}
