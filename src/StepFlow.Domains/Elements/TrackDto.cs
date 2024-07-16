using System.Collections.Generic;
using System.Drawing;
using StepFlow.Domains.Components;

namespace StepFlow.Domains.Elements
{
	public sealed class TrackDto : SubjectDto
	{
		public IList<RectangleF> Steps { get; } = new List<RectangleF>();

		public float ScaleOffset { get; set; }

		public Scale Cooldown { get; set; }
	}
}
