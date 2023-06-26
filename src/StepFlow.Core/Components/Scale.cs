﻿using System;
using System.ComponentModel;

namespace StepFlow.Core.Components
{
	public class Scale : Component, ICloneable
	{
		public Scale()
		{
		}

		public Scale(float max)
			: this(max, max)
		{
		}

		public Scale(float value, float max)
		{
			this.value = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
			this.max = max >= 0 ? max : throw new ArgumentOutOfRangeException(nameof(max));
		}

		public Scale(Scale original)
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

		public ScaleState Add(float value)
		{
			var newValue = Value + value;

			if (newValue <= 0)
			{
				Value = 0;
				return ScaleState.Min;
			}
			else if (newValue > Max)
			{
				Value = Max;
				return ScaleState.Max;
			}
			else
			{
				Value = newValue;
				return ScaleState.Node;
			}
		}

		public Scale Clone() => new Scale(this);

		object ICloneable.Clone() => Clone();
	}
}