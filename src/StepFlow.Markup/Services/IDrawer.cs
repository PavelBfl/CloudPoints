using System.Drawing;
using StepFlow.Common;
using StepFlow.Domains.Tracks;

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

		void Curve(Curve curve, Color color, int count = 10, float thickness = 2);
	}
}
