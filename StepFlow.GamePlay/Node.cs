using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.GamePlay
{
	public class Node : Core.Node, IParticle
	{
		public Node(Point position, float strengthDefault = 0)
			: base(position)
		{
			Strength = new Strength(strengthDefault);
		}

		public Strength Strength { get; }

		public IList<Command> Commands { get; } = new List<Command>();

		World IParticle.Owner => (World)OwnerRequired;
	}
}
