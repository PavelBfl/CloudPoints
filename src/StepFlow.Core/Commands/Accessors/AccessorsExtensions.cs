using System;
using System.Linq.Expressions;
using System.Reflection;

namespace StepFlow.Core.Commands.Accessors
{
	internal static class AccessorsExtensions
	{
		public static PropertyInfo GetPropertyInfo<TTarget, TValue>(Expression<Func<TTarget, TValue>> expression)
		{
			if (!(expression.Body is MemberExpression member))
			{
				throw new InvalidOperationException();
			}

			if (!(member.Member is PropertyInfo propInfo))
			{
				throw new InvalidOperationException();
			}

			if (propInfo.ReflectedType != null && typeof(TTarget) != propInfo.ReflectedType && !typeof(TTarget).IsSubclassOf(propInfo.ReflectedType))
			{
				throw new InvalidOperationException();
			}

			return propInfo;
		}

		public static IValueAccessor<TTarget, TValue> CreatePropertyAccessor<TTarget, TValue>(Expression<Func<TTarget, TValue>> expression)
		{
			if (expression is null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			var propertyInfo = GetPropertyInfo(expression);

			var getter = propertyInfo.GetGetMethod() ?? throw new InvalidOperationException();
			var setter = propertyInfo.GetSetMethod() ?? throw new InvalidOperationException();

			return new DelegateAccessor<TTarget, TValue>(
				(Func<TTarget, TValue>)getter.CreateDelegate(typeof(Func<TTarget, TValue>)),
				(Action<TTarget, TValue>)setter.CreateDelegate(typeof(Action<TTarget, TValue>))
			);
		}

		public static IBuilder<TTarget> CreatePropertyBuilder<TTarget, TValue>(Expression<Func<TTarget, TValue>> expression, TValue newValue)
		{
			if (expression is null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			var accessor = CreatePropertyAccessor(expression);

			return new ValueAccessorBuilder<TTarget, TValue>(accessor, newValue);
		}
	}
}
