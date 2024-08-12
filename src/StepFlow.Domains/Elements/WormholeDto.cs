using System.Numerics;

namespace StepFlow.Domains.Elements
{
	public enum Horizontal
	{
		Left,
		Center,
		Right,
	}

	public enum Vertical
	{
		Top,
		Center,
		Bottom,
	}

	public sealed class WormholeDto : MaterialDto
	{
		public string? Destination { get; set; }

		public Vector2 Position { get; set; }

		public Horizontal Horizontal { get; set; }

		public Vertical Vertical { get; set; }
	}
}
