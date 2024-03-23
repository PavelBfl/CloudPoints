using System;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IShapeContainerProxy : IShapeBaseProxy<ShapeContainer>, IList<Rectangle>
	{
		void Reset(IEnumerable<Rectangle> rectangles)
		{
			if (rectangles is null)
			{
				throw new ArgumentNullException();
			}

			Clear();
			foreach (var rectangle in rectangles)
			{
				Add(rectangle);
			}
		}
	}

	internal sealed class ShapeContainerProxy : ShapeBaseProxy<ShapeContainer>, IShapeContainerProxy
	{
		public ShapeContainerProxy(PlayMaster owner, ShapeContainer target) : base(owner, target)
		{
			ItemsProxy = new ListProxy<Rectangle, IList<Rectangle>>(Owner, Target);
		}

		private IList<Rectangle> ItemsProxy { get; }

		Rectangle IList<Rectangle>.this[int index] { get => ItemsProxy[index]; set => ItemsProxy[index] = value; }

		public bool IsReadOnly => ItemsProxy.IsReadOnly;

		public void Add(Rectangle item) => ItemsProxy.Add(item);

		public void Clear() => ItemsProxy.Clear();

		public bool Contains(Rectangle item) => ItemsProxy.Contains(item);

		public void CopyTo(Rectangle[] array, int arrayIndex) => ItemsProxy.CopyTo(array, arrayIndex);

		public int IndexOf(Rectangle item) => ItemsProxy.IndexOf(item);

		public void Insert(int index, Rectangle item) => ItemsProxy.Insert(index, item);

		public bool Remove(Rectangle item) => ItemsProxy.Remove(item);

		public void RemoveAt(int index) => ItemsProxy.RemoveAt(index);
	}
}
