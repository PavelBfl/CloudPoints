using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace StepFlow.Domains.Components
{
	public sealed class CollidedDto : ComponentBaseDto
	{
		public IList<Rectangle> Current { get; } = new List<Rectangle>();

		public IList<Rectangle> Next { get; } = new List<Rectangle>();

		public Vector2 Offset { get; set; }

		public bool IsMove { get; set; }

		public bool IsRigid { get; set; }
	}
}
