using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IShapeBaseProxy<out TTarget> : IProxyBase<TTarget>, IReadOnlyCollection<RectangleF>
		where TTarget : ShapeBase
	{
		RectangleF Bounds { get; }

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

		public RectangleF Bounds => Target.Bounds;

		public int Count => Target.Count;

		public void Offset(Point value) => Owner.TimeAxis.Add(new ShapeOffsetCommand(Target, value));

		public IEnumerator<RectangleF> GetEnumerator() => Target.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Register() => Owner.CreateCollectionProxy(Target.Context).Add(Target);

		public void Unregister() => Owner.CreateCollectionProxy(Target.Context).Remove(Target);
	}
}
