using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using StepFlow.Core.Commands.Accessors;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies
{
	internal class ProxyBase<TTarget> : ReadOnlyBaseProxy<TTarget>, IProxyBase<TTarget>
		where TTarget : class
	{
		public ProxyBase(PlayMaster owner, TTarget target)
			: base(owner, target)
		{
		}

		protected bool SetValue<TValue>(Expression<Func<TTarget, TValue>> expression, TValue newValue, [CallerMemberName] string? propertyName = null)
		{
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

		protected IList<TItemProxy> CreateListItemProxy<TItem, TItemProxy>(IList<TItem> target)
			where TItemProxy : IReadOnlyProxyBase<TItem>
			=> new ListItemsProxy<TItem, IList<TItem>, TItemProxy>(Owner, target);
	}
}
