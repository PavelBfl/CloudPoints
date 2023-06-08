using System;
using System.Diagnostics.CodeAnalysis;

namespace StepFlow.Core.Commands.Accessors
{
	internal class ValueAccessorBuilder<TTarget, TValue> : IBuilder<TTarget>
	{
		public ValueAccessorBuilder(IValueAccessor<TTarget, TValue> accessor)
			=> Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));

		public IValueAccessor<TTarget, TValue> Accessor { get; }

		[MaybeNull]
		[AllowNull]
		public TValue NewValue { get; set; }

		public ITargetingCommand<TTarget> Build(TTarget target) => new ValueAccessorCommand<TTarget, TValue>(target, Accessor, NewValue);

		public bool CanBuild(TTarget target) => true;
	}
}
