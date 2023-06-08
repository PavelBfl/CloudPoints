using System;

namespace StepFlow.Core.Commands.Accessors
{
	internal class ValueAccessorCommand<TTarget, TValue> : Command<TTarget>
	{
		public ValueAccessorCommand(TTarget target, IValueAccessor<TTarget, TValue> accessor, TValue newValue) : base(target)
		{
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
			NewValue = newValue;
		}

		public IValueAccessor<TTarget, TValue> Accessor { get; }

		public TValue OldValue { get; private set; }

		public TValue NewValue { get; }

		public override void Execute()
		{
			OldValue = Accessor.GetValue(Target);
			Accessor.SetValue(Target, NewValue);
		}

		public override void Revert()
		{
			Accessor.SetValue(Target, OldValue);
		}
	}
}
