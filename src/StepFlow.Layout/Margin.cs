using System;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.Layout
{
	public struct Margin : IEquatable<Margin>
	{
		private const string PROPERTIES_SEPARATOR = ", ";
		private const string NAME_VALUE_DELIMITER = ": ";
		private const string EMPTY_VIEW = "Empty";

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

		public override string ToString()
		{
			var items = new List<string>(4);

			if (Left is { } left)
			{
				items.Add(ValueToString(nameof(Left), left));
			}

			if (Right is { } right)
			{
				items.Add(ValueToString(nameof(Right), right));
			}

			if (Top is { } top)
			{
				items.Add(ValueToString(nameof(Top), top));
			}

			if (Bottom is { } bottom)
			{
				items.Add(ValueToString(nameof(Bottom), bottom));
			}

			if (items.Any())
			{
				return string.Join(PROPERTIES_SEPARATOR, items);
			}
			else
			{
				return EMPTY_VIEW;
			}
		}

		public static string ValueToString<T>(string name, T value) => name + NAME_VALUE_DELIMITER + value;
	}
}
