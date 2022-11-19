using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controlers
{
	public class RectView : PolygonBase
	{
		public RectView(Game1 game) : base(game)
		{
		}

		public RectangleF Rectangle { get; set; }

		public override IReadOnlyList<Vector2> GetVertices() => new Vector2[]
		{
			new Vector2(Rectangle.Left, Rectangle.Top),
			new Vector2(Rectangle.Right, Rectangle.Top),
			new Vector2(Rectangle.Right, Rectangle.Bottom),
			new Vector2(Rectangle.Left, Rectangle.Bottom),
		};
	}
}
