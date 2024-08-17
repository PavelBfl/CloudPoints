using System.Drawing;
using StepFlow.Common;

namespace StepFlow.Markup.Services
{
	public interface IDrawer
	{
		void DrawString(string text, PointF position, Color color);

		void DrawString(string text, RectangleF bounds, HorizontalAlign horizontalAlign, VerticalAlign verticalAlign, Color color);

		void Line(PointF start, PointF end, Color color, float thickness = 2);

		void Polygon(IReadOnlyList<PointF> vertices, Color color, float thickness = 2);

		void Draw(RectangleF rectangle, Color color);

		void Draw(Texture texture, Rectangle rectangle, Color? color = null);

		void Draw(Texture texture, RectangleF rectangle, Color? color = null);

		void Curve(PointF begin, PointF end, PointF anchor, int count, Color color, float thickness = 2);
	}
}
