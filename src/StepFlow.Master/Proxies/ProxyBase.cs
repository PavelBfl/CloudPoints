using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;

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
			if (newValue == null && oldValue == new Turn(0))
			{
				var command = Owner.GetReset(Target, accessor);
				Owner.TimeAxis.Add(command);
				return true;
			}
			else if (newValue == new Turn(1) && oldValue == null)
			{
				var command = Owner.GetWait(Target, accessor);
				Owner.TimeAxis.Add(command);
				return true;
			}
			else if (newValue is { } newValueInstance && oldValue is { } oldValueInstance && newValueInstance.Executor == oldValueInstance.Executor)
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
	}
}
