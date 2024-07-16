using System;

namespace StepFlow.Domains.Components
{
	public readonly struct Scale : IEquatable<Scale>
	{
		public static Scale CreateByMin(int max) => new Scale(0, max);

		public static Scale CreateByMax(int max) => new Scale(max, max);

		public Scale(int value, int max)
		{
			if (max < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(max));
			}

			if (value < 0 || max < value)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			Value = value;
			Max = max;
		}

		public int Value { get; }

		public int Max { get; }

		public Scale Add(int value)
		{
			var newValue = Value + value;

			if (newValue < 0)
			{
				newValue = 0;
			}
			else if (newValue > Max)
			{
				newValue = Max;
			}

			return new Scale(newValue, Max);
		}

		public Scale SetMax() => CreateByMax(Max);

		public Scale SetMin() => new Scale(0, Max);

		public Scale Increment() => Add(1);

		public Scale Decrement() => Add(-1);

		public bool IsEmpty() => Value == 0 && Max == 0;

		public bool IsMin() => Value == 0 && Max != 0;

		public bool IsMax() => Value == Max && Max != 0;

		public bool Equals(Scale other) => Value == other.Value && Max == other.Max;

		public override string ToString() => Value + "/" + Max;

		public override bool Equals(object obj) => obj is Scale instance && Equals(instance);

		public override int GetHashCode() => HashCode.Combine(Value, Max);

		public static Scale operator ++(Scale scale) => scale.Increment();

		public static Scale operator --(Scale scale) => scale.Decrement();

		public static Scale operator +(Scale scale, int value) => scale.Add(value);

		public static Scale operator -(Scale scale, int value) => scale.Add(-value);

		public static bool operator ==(Scale left, Scale right) => left.Equals(right);

		public static bool operator !=(Scale left, Scale right) => !(left == right);
	}
}
