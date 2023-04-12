using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public class Node : Particle
	{
		private const string VIEW_FORMAT = "{0}:{1}";

		public Node(Point position)
		{
			Position = position;
		}

		public Point Position { get; }

		public OccupiersCollection Occupiers { get; } = new OccupiersCollection();

		protected override void OnOwnerChange()
		{
			base.OnOwnerChange();

			foreach (var occupier in Occupiers.ToArray())
			{
				occupier.Current = null;
			}
		}

		public override string ToString() => string.Format(VIEW_FORMAT, Position.X, Position.Y);
	}
}
