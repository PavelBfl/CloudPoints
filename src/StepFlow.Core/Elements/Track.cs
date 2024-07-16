using System.Collections.Generic;
using System.Drawing;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Track : Subject
	{
		public Track()
		{
		}

		public Track(TrackDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);

			Steps.AddRange(original.Steps);
			ScaleOffset = original.ScaleOffset;
			Cooldown = original.Cooldown;
		}

		public IList<RectangleF> Steps { get; } = new List<RectangleF>();

		public float ScaleOffset { get; set; }

		public Scale Cooldown { get; set; }
	}
}
