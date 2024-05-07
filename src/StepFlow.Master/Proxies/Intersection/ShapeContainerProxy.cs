using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IShapeContainerProxy : IShapeBaseProxy<ShapeContainer>, ICollection<RectangleF>
	{
		void Reset(RectangleF rectangle, bool force = false)
		{
			if (force || !Equal(new[] { rectangle }))
			{
				Clear();
				Add(rectangle);
			}
		}

		void Reset(IEnumerable<RectangleF> rectangles, bool force = false)
		{
			if (rectangles is null)
			{
				throw new ArgumentNullException(nameof(rectangles));
			}

			if (force || !Equal(rectangles.ToArray()))
			{
				Clear();
				foreach (var rectangle in rectangles)
				{
					Add(rectangle);
				}
			}
		}

		bool Equal(ICollection<RectangleF> rectangles)
		{
			if (rectangles is null)
			{
				throw new ArgumentNullException(nameof(rectangles));
			}

			if (rectangles.Count != Target.Count)
			{
				return false;
			}

			foreach (var rectangle in rectangles)
			{
				if (!Target.Contains(rectangle))
				{
					return false;
				}
			}

			return true;
		}
	}

	internal sealed class ShapeContainerProxy : ShapeBaseProxy<ShapeContainer>, IShapeContainerProxy
	{
		public ShapeContainerProxy(PlayMaster owner, ShapeContainer target) : base(owner, target)
		{
			ItemsProxy = Owner.CreateCollectionProxy(Target);
		}

		private ICollection<RectangleF> ItemsProxy { get; }

		public bool IsReadOnly => ItemsProxy.IsReadOnly;

		public void Add(RectangleF item) => ItemsProxy.Add(item);

		public void Clear() => ItemsProxy.Clear();

		public bool Contains(RectangleF item) => ItemsProxy.Contains(item);

		public void CopyTo(RectangleF[] array, int arrayIndex) => ItemsProxy.CopyTo(array, arrayIndex);

		public bool Remove(RectangleF item) => ItemsProxy.Remove(item);
	}
}
