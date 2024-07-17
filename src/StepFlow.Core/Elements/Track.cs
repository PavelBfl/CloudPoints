using System.Collections.Generic;
using System.Drawing;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Track : Subject
	{
		public Track(IContext context)
			: base(context)
		{
		}

		public Track(IContext context, TrackDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Steps.AddRange(original.Steps);
			ScaleOffset = original.ScaleOffset;
			Cooldown = original.Cooldown;
		}

		public IList<RectangleF> Steps { get; } = new List<RectangleF>();

		public float ScaleOffset { get; set; }

		public Scale Cooldown { get; set; }

		public override SubjectDto ToDto()
		{
			var result = new TrackDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(TrackDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Steps.AddRange(Steps);
			container.ScaleOffset = ScaleOffset;
			container.Cooldown = Cooldown;
		}
	}
}
