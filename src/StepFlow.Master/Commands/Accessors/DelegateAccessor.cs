using System;

namespace StepFlow.Core.Commands.Accessors
{
	internal sealed class DelegateAccessor<TTarget, TValue> : IValueAccessor<TTarget, TValue>
	{
		public DelegateAccessor(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
		{
			Getter = getter ?? throw new ArgumentNullException(nameof(getter));
			Setter = setter ?? throw new ArgumentNullException(nameof(setter));
		}

		private Func<TTarget, TValue> Getter { get; }

		private Action<TTarget, TValue> Setter { get; }

		public TValue GetValue(TTarget target) => Getter(target);

		public void SetValue(TTarget target, TValue value) => Setter(target, value);
	}
}
