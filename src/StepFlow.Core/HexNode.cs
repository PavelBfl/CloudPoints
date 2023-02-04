using System.Drawing;

namespace StepFlow.Core
{
	public class HexNode : Particle
	{
		private const string VIEW_FORMAT = "{0}:{1}";

		public HexNode(World? owner, Point posititon)
			: base(owner)
		{
			Position = posititon;
		}

		public Point Position { get; }

		public OccupiersCollection Occupiers { get; } = new OccupiersCollection();

		public override string ToString() => string.Format(VIEW_FORMAT, Position.X, Position.Y);
	}
}
