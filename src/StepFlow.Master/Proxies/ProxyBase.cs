using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StepFlow.Core.Commands.Accessors;

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
	}
}
