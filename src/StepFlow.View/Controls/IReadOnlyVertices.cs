using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public interface IReadOnlyVertices : IReadOnlyList<Vector2>
	{
		public static IReadOnlyVertices Empty => EmptyVertices.Instance;

		System.Drawing.RectangleF Bounds { get; }

		bool FillContains(Point point);
	}
}
