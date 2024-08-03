using System.Numerics;

namespace StepFlow.Domains.Elements
{
	public sealed class WormholeDto : MaterialDto
	{
		public string? Destination { get; set; }

		public Vector2 Position { get; set; }
	}
}
