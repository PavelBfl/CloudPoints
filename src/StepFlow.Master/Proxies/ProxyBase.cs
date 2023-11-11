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

		protected void SetValue<TValue>(Expression<Func<TTarget, TValue>> expression, TValue newValue, [CallerMemberName] string? propertyName = null)
		{
			var command = Target.CreatePropertyCommand(expression, newValue);
			Owner.TimeAxis.Add(command);
		}

		protected IList<T> CreateListProxy<T>(IList<T> target) => new ListProxy<T, IList<T>>(Owner, target);
	}
}
