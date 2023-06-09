using System;

namespace StepFlow.Core.Commands.Accessors
{
	internal class ValueAccessorBuilder<TTarget, TValue> : IBuilder<TTarget>
	{
		public ValueAccessorBuilder(IValueAccessor<TTarget, TValue> accessor, TValue newValue, IResolver<TTarget> resolver)
		{
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
			NewValue = newValue;
			Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
		}

		public IValueAccessor<TTarget, TValue> Accessor { get; }

		public TValue NewValue { get; }

		public IResolver<TTarget> Resolver { get; }

		public ITargetingCommand<TTarget> Build(TTarget target) => new ValueAccessorCommand<TTarget, TValue>(target, Accessor, NewValue, Resolver);

		public bool CanBuild(TTarget target) => true;
	}
}
