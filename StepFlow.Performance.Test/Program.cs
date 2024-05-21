using System.Drawing;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Markup;
using StepFlow.Markup.Services;
using StepFlow.Master.Proxies.Elements;

public class Program
{
	private static void Main(string[] args)
	{
		var handler = new GameHandler(
			new RectangleF(0, 0, 1000, 1000),
			new Drawer(),
			new Control()
		);

		const int FRAMES_COUNT = 10000;
		for (var i = 0; i < FRAMES_COUNT; i++)
		{
			handler.Update();
		}
	}

	private sealed class Control : IControl
	{
		public PlayerAction GetPlayerAction() => PlayerAction.None;

		public float? GetPlayerCourse() => null;

		public float GetPlayerRotate(Vector2 center) => 0;

		public TimeOffset GetTimeOffset() => TimeOffset.None;

		public bool IsUndo() => false;

		public bool OnSwitchDebug() => false;
	}

	private sealed class Drawer : IDrawer
	{
		public void Draw(Texture texture, Rectangle rectangle, Color? color = null)
		{
		}

		public void Draw(Texture texture, RectangleF rectangle, Color? color = null)
		{
		}

		public void DrawString(string text, PointF position, Color color)
		{
		}

		public void DrawString(string text, RectangleF bounds, HorizontalAlign horizontalAlign, VerticalAlign verticalAlign, Color color)
		{
		}

		public void Line(PointF start, PointF end, Color color, float thickness = 2)
		{
		}

		public void Polygon(IReadOnlyList<PointF> vertices, Color color, float thickness = 2)
		{
		}
	}
}