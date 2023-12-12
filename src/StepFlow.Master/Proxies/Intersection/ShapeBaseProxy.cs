using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IShapeBaseProxy<out TTarget> : IProxyBase<TTarget>, IReadOnlyList<Rectangle>
		where TTarget : ShapeBase
	{
		Rectangle Bounds { get; }
	}

	internal class ShapeBaseProxy<TTarget> : ProxyBase<TTarget>, IShapeBaseProxy<TTarget>
		where TTarget : ShapeBase
	{
		public ShapeBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public Rectangle this[int index] => Target[index];

		public Rectangle Bounds => Target.Bounds;

		public int Count => Target.Count;

		public IEnumerator<Rectangle> GetEnumerator() => Target.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
