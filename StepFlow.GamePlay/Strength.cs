using System;

namespace StepFlow.GamePlay
{
	public class Strength : ICloneable
	{
		public Strength(float max)
			: this(max, max)
		{
		}

		public Strength(float value, float max)
		{
			this.value = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
			this.max = max >= 0 ? max : throw new ArgumentOutOfRangeException(nameof(max));
		}

		public Strength(Strength original)
			: this(
				(original ?? throw new ArgumentNullException(nameof(original))).Value,
				(original ?? throw new ArgumentNullException(nameof(original))).Max
			)
		{
		}

		private float value;

		public float Value
		{
			get => value;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this.value = MathF.Min(value, Max);
			}
		}

		private float max;

		public float Max
		{
			get => max;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				max = value;
				Value = MathF.Min(Value, Max);
			}
		}

		public StrengthState Add(float value)
		{
			var newValue = Value + value;

			if (newValue <= 0)
			{
				Value = 0;
				return StrengthState.Min;
			}
			else if (newValue > Max)
			{
				Value = Max;
				return StrengthState.Max;
			}
			else
			{
				Value = newValue;
				return StrengthState.Node;
			}
		}

		public Strength Clone() => new Strength(this);

		object ICloneable.Clone() => Clone();
	}
}
