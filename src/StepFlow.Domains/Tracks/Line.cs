using System.Numerics;

namespace StepFlow.Domains.Tracks
{
	public struct Line
	{
		public Vector2 Begin { get; set; }

		public Vector2 End { get; set; }

		public readonly float GetLength() => Vector2.Distance(Begin, End);
	}
}
