using System.Drawing;

namespace StepFlow.Core
{
	public class Node : Particle
	{
		private const string VIEW_FORMAT = "{0}:{1}";

		public Node(World? owner, Point position)
			: base(owner)
		{
			Position = position;
		}

		public override World? Owner
		{
			get => base.Owner;
			set
			{
				if (Owner != value)
				{
					if (Owner is { })
					{
						Owner.Place.Remove(Position);
					}

					base.Owner = value;

					if (Owner is { })
					{
						Owner.Place.Add(this);
					}
				}
			}
		}

		public Point Position { get; }

		public OccupiersCollection Occupiers { get; } = new OccupiersCollection();

		public override string ToString() => string.Format(VIEW_FORMAT, Position.X, Position.Y);
	}
}
