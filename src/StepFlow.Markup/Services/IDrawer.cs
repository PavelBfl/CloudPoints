using System.Drawing;
using StepFlow.View.Controls;

namespace StepFlow.Markup.Services
{
	public interface IDrawer
	{
		void DrawString(string text, PointF position, Color color);

		void DrawString(string text, RectangleF bounds, HorizontalAlign horizontalAlign, VerticalAlign verticalAlign, Color color);

		void Line(PointF start, PointF end, Color color, float thickness = 2);

		void Polygon(IReadOnlyList<PointF> vertices, Color color, float thickness = 2);

		void Draw(Texture texture, Rectangle rectangle, Color? color = null);
	}
}
