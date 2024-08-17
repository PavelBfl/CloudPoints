using System.Numerics;
using StepFlow.Common;

namespace StepFlow.Domains.Elements
{
	public sealed class WormholeDto : MaterialDto
	{
		public string? Destination { get; set; }

		public Vector2 Position { get; set; }

		public HorizontalAlign Horizontal { get; set; }

		public VerticalAlign Vertical { get; set; }
	}
}
