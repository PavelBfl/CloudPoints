using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Track : Subject
	{
		public IList<RectangleF> Steps { get; } = new List<RectangleF>();

		public float ScaleOffset { get; set; }

		public Scale? Cooldown { get; set; }

		public Scale GetCooldownRequired() => PropertyRequired(Cooldown);
	}
}
