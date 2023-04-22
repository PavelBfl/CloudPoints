using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public class BoundsVertices : IReadOnlyVertices
	{
		private Vector2[]? items;

		public Vector2[] Items => items ??= new Vector2[]
		{
			new Vector2(Bounds.Left, Bounds.Top),
			new Vector2(Bounds.Right, Bounds.Top),
			new Vector2(Bounds.Right, Bounds.Bottom),
			new Vector2(Bounds.Left, Bounds.Bottom),
		};

		private RectangleF bounds;

		public RectangleF Bounds
		{
			get => bounds;
			set
			{
				if (Bounds != value)
				{
					bounds = value;
					items = null;
				}
			}
		}

		public int Count => 4;

		public Vector2 this[int index] => Items[index];

		public bool FillContains(Microsoft.Xna.Framework.Point point) => Bounds.Contains(point.X, point.Y);

		public IEnumerator<Vector2> GetEnumerator() => Items.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
