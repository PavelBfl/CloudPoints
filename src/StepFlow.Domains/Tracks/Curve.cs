using System;
using System.Collections.Generic;
using System.Numerics;

namespace StepFlow.Domains.Tracks
{
	public struct Curve : IEquatable<Curve>
	{
		public static Vector2 GetPoint(Vector2 begin, Vector2 end, Vector2 control, float amount)
		{
			var beginToControl = Vector2.Lerp(begin, control, amount);
			var controlToEnd = Vector2.Lerp(control, end, amount);
			return Vector2.Lerp(beginToControl, controlToEnd, amount);
		}

		public readonly Vector2 GetPoint(float amount)
		{
			var beginToBeginControl = Vector2.Lerp(Begin, BeginControl, amount);
			var beginControlToEndControl = Vector2.Lerp(BeginControl, EndControl, amount);
			var endControlToEnd = Vector2.Lerp(EndControl, End, amount);

			return GetPoint(beginToBeginControl, endControlToEnd, beginControlToEndControl, amount);
		}

		public readonly IEnumerable<Line> GetLines()
		{
			var stepsCount = (int)(Vector2.Distance(Begin, BeginControl) +
				Vector2.Distance(BeginControl, EndControl) +
				Vector2.Distance(EndControl, End));

			var prev = Begin;
			for (var i = 0; i <= stepsCount; i++)
			{
				var amount = (float)i / stepsCount;
				var current = GetPoint(amount);
				yield return new Line() { Begin = prev, End = current };
				prev = current;
			}
		}

		public Vector2 Begin { get; set; }

		public Vector2 End { get; set; }

		public Vector2 BeginControl { get; set; }

		public Vector2 EndControl { get; set; }

		public Curve Transform(Matrix3x2 matrix) => new Curve()
		{
			Begin = Vector2.Transform(Begin, matrix),
			BeginControl = Vector2.Transform(BeginControl, matrix),
			EndControl = Vector2.Transform(EndControl, matrix),
			End = Vector2.Transform(End, matrix),
		};

		public readonly bool Equals(Curve other)
			=> Begin == other.Begin && End == other.End && BeginControl == other.BeginControl && EndControl == other.EndControl;

		public readonly override bool Equals(object obj) => obj is Curve curve && Equals(curve);

		public readonly override int GetHashCode() => HashCode.Combine(Begin, End, BeginControl, EndControl);

		public static bool operator ==(Curve left, Curve right) => left.Equals(right);

		public static bool operator !=(Curve left, Curve right) => !(left == right);
	}
}
