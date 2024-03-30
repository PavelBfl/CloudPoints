﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IShapeBaseProxy<out TTarget> : IProxyBase<TTarget>, IReadOnlyCollection<Rectangle>
		where TTarget : ShapeBase
	{
		Rectangle Bounds { get; }

		void Offset(Point value);
		void Register();
		void Unregister();
	}

	internal class ShapeBaseProxy<TTarget> : ProxyBase<TTarget>, IShapeBaseProxy<TTarget>
		where TTarget : ShapeBase
	{
		public ShapeBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public Rectangle Bounds => Target.Bounds;

		public int Count => Target.Count;

		public void Offset(Point value) => Owner.TimeAxis.Add(new ShapeOffsetCommand(Target, value));

		public IEnumerator<Rectangle> GetEnumerator() => Target.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Register() => Owner.CreateCollectionProxy(Playground.IntersectionContext).Add(Target);

		public void Unregister() => Owner.CreateCollectionProxy(Playground.IntersectionContext).Remove(Target);
	}
}
