﻿using System;
using StepFlow.TimeLine;

namespace StepFlow.Core.Commands.Accessors
{
	internal class ValueAccessorCommand<TTarget, TValue> : ICommand
	{
		public ValueAccessorCommand(TTarget target, IValueAccessor<TTarget, TValue> accessor, TValue newValue, IResolver<TTarget> resolver)
		{
			Target = target;
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
			NewValue = newValue;
		}

		public IValueAccessor<TTarget, TValue> Accessor { get; }

		public TTarget Target { get; }

		public TValue OldValue { get; private set; }

		public TValue NewValue { get; }

		public void Execute()
		{
			OldValue = Accessor.GetValue(Target);
			Accessor.SetValue(Target, NewValue);
		}

		public void Revert()
		{
			Accessor.SetValue(Target, OldValue);
		}
	}
}