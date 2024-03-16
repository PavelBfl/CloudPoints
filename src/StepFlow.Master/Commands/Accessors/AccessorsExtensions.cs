using System;
using System.Linq.Expressions;
using System.Reflection;
using StepFlow.TimeLine;

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
				// TODO перестроить условие на IsAssignableFrom
				//throw new InvalidOperationException();
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

			return CreatePropertyAccessor<TTarget, TValue>(propertyInfo);
		}

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

		public static ICommand CreatePropertyCommand<TTarget, TValue>(this TTarget target, Expression<Func<TTarget, TValue>> expression, TValue newValue)
		{
			var accessor = CreatePropertyAccessor(expression);

			return new ValueAccessorCommand<TTarget, TValue>(target, accessor, newValue);
		}
	}
}
