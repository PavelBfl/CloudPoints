using System;
using StepFlow.TimeLine;

namespace StepFlow.Core.Commands.Accessors
{
	internal sealed class ValueAccessorCommand<TTarget, TValue> : ICommand
	{
		public ValueAccessorCommand(TTarget target, IValueAccessor<TTarget, TValue> accessor, TValue newValue)
		{
			Target = target;
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
			NewValue = newValue;
			OldValue = Accessor.GetValue(Target);
		}

		public IValueAccessor<TTarget, TValue> Accessor { get; }

		public TTarget Target { get; }

		public TValue OldValue { get; }

		public TValue NewValue { get; }

		public void Execute() => Accessor.SetValue(Target, NewValue);

		public void Revert() => Accessor.SetValue(Target, OldValue);
	}
}
