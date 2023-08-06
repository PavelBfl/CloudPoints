using System.ComponentModel;
using System.Drawing;

namespace StepFlow.Core.Components
{
	public sealed class Collided : Component
	{
		public IBorderedNode? Border { get; set; }

		public Point Offset { get; set; }

		public void Move()
		{
			if (Border is { } border)
			{
				Border.Offset(Offset);
				Offset = Point.Empty;
			}
		}
	}
}
