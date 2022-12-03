using System;
using System.Data.Common;

namespace StepFlow.Layout
{
	public struct Margin : IEquatable<Margin>
	{
		public Margin(float all)
			: this(all, all, all, all)
		{
		}

		public Margin(float? left, float? right, float? top, float? bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		public float? Left { get; set; }
		public float? Right { get; set; }
		public float? Top { get; set; }
		public float? Bottom { get; set; }

		public bool Equals(Margin other) => Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;

		public override bool Equals(object obj) => obj is Margin other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(Left, Right, Top, Bottom);

		public static bool operator ==(Margin left, Margin right) => left.Equals(right);

		public static bool operator !=(Margin left, Margin right) => !(left == right);
	}
}
