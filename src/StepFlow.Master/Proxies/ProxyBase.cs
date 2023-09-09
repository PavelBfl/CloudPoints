﻿using System;
using System.Linq.Expressions;
using StepFlow.Core.Commands.Accessors;

namespace StepFlow.Master.Proxies
{
	public class ProxyBase<TTarget> : IProxyBase<TTarget>
		where TTarget : class
	{
		public ProxyBase(PlayMaster owner, TTarget target)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Target = target ?? throw new ArgumentNullException(nameof(target));
		}

		public PlayMaster Owner { get; }

		public TTarget Target { get; }

		protected void SetValue<TValue>(Expression<Func<TTarget, TValue>> expression, TValue newValue)
		{
			var command = Target.CreatePropertyCommand(expression, newValue);
			Owner.TimeAxis.Add(command);
		}
	}
}
