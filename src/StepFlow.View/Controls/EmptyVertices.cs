using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	internal sealed class EmptyVertices : IReadOnlyVertices
	{
		public static EmptyVertices Instance { get; } = new EmptyVertices();

		private EmptyVertices()
		{
		}

		public Vector2 this[int index] => throw new IndexOutOfRangeException();

		public System.Drawing.RectangleF Bounds => System.Drawing.RectangleF.Empty;

		public int Count => 0;

		public bool FillContains(Point point) => false;

		public IEnumerator<Vector2> GetEnumerator() => Enumerable.Empty<Vector2>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
