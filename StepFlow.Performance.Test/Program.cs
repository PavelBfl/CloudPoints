using System.Diagnostics;
using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using StepFlow.Core;
using StepFlow.Markup;
using StepFlow.Markup.Services;

public class Program
{
	private static void Main(string[] args)
	{
		var handler = new GameHandler(
			new(0, 0, 800, 480),
			new Drawer(),
			new Keyboard()
		);

		var sw = Stopwatch.StartNew();
		handler.Update();

		const int COUNT = 100;
		for (var i = 0; i < COUNT; i++)
		{
			handler.Update();
		}

		Console.WriteLine(sw.Elapsed / COUNT);
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

	private sealed class Keyboard : IKeyboard
	{
		private ulong counter = 0;

		public Course? GetPlayerCourse() => (Course)(counter++ % 8);

		public Course? GetPlayerShot() => null;

		public bool IsUndo() => false;
	}
}