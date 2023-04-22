using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public class VerticesCollection : IList<Vector2>, IReadOnlyVertices
	{
		public VerticesCollection()
		{
		}

		public VerticesCollection(IEnumerable<Vector2> items)
		{
			Items.AddRange(items);
		}

		public Vector2 this[int index]
		{
			get => Items[index];
			set
			{
				Items[index] = value;
				Refresh();
			}
		}

		public int Count => Items.Count;

		public bool IsReadOnly => false;

		public System.Drawing.RectangleF? bounds;

		public System.Drawing.RectangleF Bounds
		{
			get
			{
				if (bounds is null)
				{
					var min = new Vector2(float.MaxValue);
					var max = new Vector2(float.MinValue);

					foreach (var vertex in this)
					{
						min.X = MathF.Min(min.X, vertex.X);
						min.Y = MathF.Min(min.Y, vertex.Y);
						max.X = MathF.Max(max.X, vertex.X);
						max.Y = MathF.Max(max.Y, vertex.Y);
					}

					bounds = System.Drawing.RectangleF.FromLTRB(min.X, min.Y, max.X, max.Y);
				}

				return bounds.Value;
			}
		}

		private List<Vector2> Items { get; } = new List<Vector2>();

		private void Refresh()
		{
			bounds = null;
		}

		public void Add(Vector2 item)
		{
			Items.Add(item);
			Refresh();
		}

		public void Clear()
		{
			Items.Clear();
			Refresh();
		}

		public bool Contains(Vector2 item) => Items.Contains(item);

		public void CopyTo(Vector2[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

		public IEnumerator<Vector2> GetEnumerator() => Items.GetEnumerator();

		public int IndexOf(Vector2 item) => Items.IndexOf(item);

		public void Insert(int index, Vector2 item)
		{
			Items.Insert(index, item);
			Refresh();
		}

		public bool Remove(Vector2 item)
		{
			var removed = Items.Remove(item);

			if (removed)
			{
				Refresh();
			}

			return removed;
		}

		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
			Refresh();
		}

		IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

		public bool FillContains(Point point) => Bounds.Contains(point.X, point.Y) && Utils.Contains(this, point.ToVector2());
	}
}
