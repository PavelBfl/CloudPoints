using System;

namespace StepFlow.Core.Commands.Accessors
{
	internal class ValueAccessorBuilder<TTarget, TValue> : IBuilder<TTarget>
	{
		public ValueAccessorBuilder(IValueAccessor<TTarget, TValue> accessor, TValue newValue)
		{
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
			NewValue = newValue;
		}

		public IValueAccessor<TTarget, TValue> Accessor { get; }

		public TValue NewValue { get; }

		public ITargetingCommand<TTarget> Build(TTarget target) => new ValueAccessorCommand<TTarget, TValue>(target, Accessor, NewValue);

		public bool CanBuild(TTarget target) => true;
	}
}
