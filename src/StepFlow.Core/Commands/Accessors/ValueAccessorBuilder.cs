using System;

namespace StepFlow.Core.Commands.Accessors
{
	internal class ValueAccessorBuilder<TTarget, TValue> : IBuilder<TTarget>
	{
		public ValueAccessorBuilder(IValueAccessor<TTarget, TValue> accessor, TValue newValue, IResolverBuilder<TTarget> resolverBuilder)
		{
			Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
			NewValue = newValue;
			ResolverBuilder = resolverBuilder ?? throw new ArgumentNullException(nameof(resolverBuilder));
		}

		public IValueAccessor<TTarget, TValue> Accessor { get; }

		public TValue NewValue { get; }

		public IResolverBuilder<TTarget> ResolverBuilder { get; }

		public ITargetingCommand<TTarget> Build(TTarget target)
			=> new ValueAccessorCommand<TTarget, TValue>(target, Accessor, NewValue, ResolverBuilder.Build(target));

		public bool CanBuild(TTarget target) => true;
	}
}
