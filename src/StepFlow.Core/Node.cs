using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core.Commands;

namespace StepFlow.Core
{
	public class Node : Particle
	{
		private const string VIEW_FORMAT = "{0}:{1}";

		public Node(Playground owner, Point position) : base(owner)
		{
			Position = position;
		}

		public Point Position { get; }

		public OccupiersCollection Occupiers { get; } = new OccupiersCollection();

		public IList<ITargetingCommand<Node>> Commands { get; } = new List<ITargetingCommand<Node>>();

		public override string ToString() => string.Format(VIEW_FORMAT, Position.X, Position.Y);
	}
}
