using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IShapeProxy : IProxyBase<Shape>, ICollection<Rectangle>
	{
		Rectangle Bounds { get; }

		void Offset(Point value);

		void Reset(Rectangle rectangle, bool force = false)
		{
			if (force || !Equal(new[] { rectangle }))
			{
				Clear();
				Add(rectangle);
			}
		}

		void Reset(IEnumerable<Rectangle> rectangles, bool force = false)
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

		bool Equal(ICollection<Rectangle> rectangles)
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

	internal sealed class ShapeProxy : CollectionProxy<Rectangle, Shape>, IShapeProxy
	{
		public ShapeProxy(PlayMaster owner, Shape target) : base(owner, target)
		{
		}

		public Rectangle Bounds => Target.Bounds;

		public int Count => Target.Count;

		public void Offset(Point value) => Owner.TimeAxis.Add(new ShapeOffsetCommand(Target, value));

		public IEnumerator<Rectangle> GetEnumerator() => Target.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
