using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;
using StepFlow.Master.Commands;
using StepFlow.Master.Proxies.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies
{
	internal class ProxyBase<TTarget> : IProxyBase<TTarget>
		where TTarget : class
	{
		public ProxyBase(PlayMaster owner, TTarget target)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Target = target ?? throw new ArgumentNullException(nameof(target));
		}

		public PlayMaster Owner { get; }

		public TTarget Target { get; }

		protected bool SetValue(Turn? newValue, [CallerMemberName] string? propertyName = null)
		{
			if (propertyName is null)
			{
				throw new ArgumentNullException(nameof(propertyName));
			}

			var accessor = Owner.GetAccessor<TTarget, Turn?>(propertyName);

			var oldValue = accessor.GetValue(Target);
			if (newValue is { } newValueInstance && oldValue is { } oldValueInstance && newValueInstance.Executor == oldValueInstance.Executor)
			{
				if (newValueInstance.Duration == oldValueInstance.Duration - 1)
				{
					var command = Owner.GetDecrement(Target, accessor);
					Owner.TimeAxis.Add(command);
					return true;
				}
				else if (newValueInstance.Duration == oldValueInstance.Duration + 1)
				{
					var command = Owner.GetIncrement(Target, accessor);
					Owner.TimeAxis.Add(command);
					return true;
				}
			}

			return SetValue<Turn?>(newValue, propertyName);
		}

		protected bool SetValue<TValue>(TValue newValue, [CallerMemberName] string? propertyName = null)
		{
			if (propertyName is null)
			{
				throw new ArgumentNullException(nameof(propertyName));
			}

			var accessor = Owner.GetAccessor<TTarget, TValue>(propertyName);

			var change = !EqualityComparer<TValue>.Default.Equals(accessor.GetValue(Target), newValue);
			if (change)
			{
				var command = new ValueAccessorCommand<TTarget, TValue>(Target, accessor, newValue);
				Owner.TimeAxis.Add(command);
			}

			return change;
		}

		protected bool SetValue<TValue>(Expression<Func<TTarget, TValue>> expression, TValue newValue, [CallerMemberName] string? propertyName = null)
		{
			var propertyInfo = AccessorsExtensions.GetPropertyInfo(expression);

			return SetValue(newValue, propertyInfo.Name);

			var accessor = AccessorsExtensions.CreatePropertyAccessor(expression);

			var change = !EqualityComparer<TValue>.Default.Equals(accessor.GetValue(Target), newValue);
			if (change)
			{
				var command = new ValueAccessorCommand<TTarget, TValue>(Target, accessor, newValue);
				Owner.TimeAxis.Add(command);
			}

			return change;
		}

		protected IList<T> CreateListProxy<T>(IList<T> target) => new ListProxy<T, IList<T>>(Owner, target);

		protected ICollection<T> CreateCollectionProxy<T>(ICollection<T> target) => new CollectionProxy<T, ICollection<T>>(Owner, target);
	}
}
