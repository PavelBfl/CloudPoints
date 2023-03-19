using System;

namespace StepFlow.GamePlay
{
	public class Strength
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
	}
}
