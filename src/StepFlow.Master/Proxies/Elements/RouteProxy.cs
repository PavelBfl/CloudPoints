﻿using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IRouteProxy : IProxyBase<Route>
	{
		float Offset { get; set; }
		float Speed { get; set; }
	}

	internal class RouteProxy : ProxyBase<Route>, IRouteProxy
	{
		public RouteProxy(PlayMaster owner, Route target) : base(owner, target)
		{
		}

		public float Offset { get => Target.Offset; set => SetValue(value); }

		public float Speed { get => Target.Speed; set => SetValue(value); }
	}
}
