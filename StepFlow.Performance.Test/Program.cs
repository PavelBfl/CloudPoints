using System.Drawing;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Markup;
using StepFlow.Markup.Services;

public class Program
{
	private static void Main(string[] args)
	{
		var keyboard = new Keyboard()
		{
			PlayerShot = Course.Left,
		};

		var handler = new GameHandler(
			new RectangleF(0, 0, 1000, 1000),
			new Drawer(),
			keyboard
		);

		const int FRAMES_COUNT = 10000;
		var sw = new System.Diagnostics.Stopwatch();
		for (var i = 0; i < FRAMES_COUNT; i++)
		{
			var range = i / 120;
			keyboard.PlayerCourse = range % 2 == 0 ? Course.Right : Course.Left;

			sw.Start();
			handler.Update();
			sw.Stop();
			Console.WriteLine(i + ": " + sw.Elapsed);
			sw.Reset();
		}
	}

	private sealed class Keyboard : IControl
	{
		public Course? PlayerCourse { get; set; }

		public Course? PlayerShot { get; set; }

		public PlayerAction GetPlayerAction()
		{
			throw new NotImplementedException();
		}

		public Course? GetPlayerCourse() => PlayerCourse;

		public float GetPlayerRotate(Vector2 center)
		{
			throw new NotImplementedException();
		}

		public Course? GetPlayerShot() => PlayerShot;

		public TimeOffset GetTimeOffset() => TimeOffset.None;

		public bool IsUndo() => false;

		public bool OnSwitchDebug() => false;
	}

	private sealed class Drawer : IDrawer
	{
		public void Draw(Texture texture, Rectangle rectangle, Color? color = null)
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