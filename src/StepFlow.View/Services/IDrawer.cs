using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.View.Controls;

namespace StepFlow.View.Services
{
	public interface IDrawer
	{
		// TODO Временно вынесено, удалить в будущем
		SpriteBatch SpriteBatch { get; }

		void DrawString(string text, Vector2 position, Color color);

		void DrawString(string text, System.Drawing.RectangleF bounds, HorizontalAlign horizontalAlign, VerticalAlign verticalAlign, Color color);

		Vector2 MeasureString(string text);

		void Line(Vector2 start, Vector2 end, Color color, float thickness = 2);

		void Polygon(IReadOnlyList<Vector2> vertices, Color color, float thickness = 2);
	}
}
