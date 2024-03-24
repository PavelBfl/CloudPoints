using System;
using System.Reflection;

namespace StepFlow.Core.Commands.Accessors
{
	internal static class AccessorsExtensions
	{
		public static IValueAccessor<TTarget, TValue> CreatePropertyAccessor<TTarget, TValue>(PropertyInfo propertyInfo)
		{
			if (propertyInfo is null)
			{
				throw new ArgumentNullException(nameof(propertyInfo));
			}

			var getter = propertyInfo.GetGetMethod() ?? throw new InvalidOperationException();
			var setter = propertyInfo.GetSetMethod() ?? throw new InvalidOperationException();

			return new DelegateAccessor<TTarget, TValue>(
				(Func<TTarget, TValue>)getter.CreateDelegate(typeof(Func<TTarget, TValue>)),
				(Action<TTarget, TValue>)setter.CreateDelegate(typeof(Action<TTarget, TValue>))
			);
		}
	}
}
