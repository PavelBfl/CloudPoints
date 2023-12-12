using System.Drawing;

namespace StepFlow.Intersection
{
	public struct RectangleMask
	{
		public RectangleMask(Rectangle value)
		{
			Value = value;

			var location = new Point(
				Value.Location.X / 32,
				Value.Location.Y / 32
			);

			var size = new Size(
				Value.Size.Width / 32 + 1,
				Value.Size.Height / 32 + 1
			);

			var horizontal = 0;
			for (var iX = 0; iX < size.Width; iX++)
			{
				horizontal |= 1 << (iX + location.X);
			}

			var vertical = 0;
			for (var iY = 0; iY < size.Height; iY++)
			{
				vertical |= 1 << (iY + location.Y);
			}

			Mask = new Point(horizontal, vertical);
		}

		public Rectangle Value;

		public Point Mask;

		public bool IntersectWith(ref RectangleMask other)
		{
			return ((Mask.X & other.Mask.X) | (Mask.Y & other.Mask.Y)) != 0 && Value.IntersectsWith(other.Value);
		}
	}
}
