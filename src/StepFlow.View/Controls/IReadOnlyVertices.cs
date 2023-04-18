using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public interface IReadOnlyVertices : IReadOnlyList<Vector2>
	{
		System.Drawing.RectangleF Bounds { get; }
	}
}
