using System.Collections.Immutable;
using System.Drawing;

namespace StepFlow.Intersection.Test;

internal static class Extensions
{
	private static Rectangle CreateRectangle(Random random)
	{
		const int MIN_X = -100;
		const int MAX_X = 100;
		const int MIN_Y = -100;
		const int MAX_Y = -100;
		const int MAX_WIDTH = 100;
		const int MAX_HEIGHT = 100;


		return new(
			random.Next(MIN_X, MAX_X),
			random.Next(MIN_Y, MAX_Y),
			random.Next(1, MAX_WIDTH),
			random.Next(1, MAX_HEIGHT)
		);
	}

	private static IEnumerable<Rectangle> CreateRectangles(Random random)
	{
		const int MAX_COUNT = 10;

		var count = random.Next(1, MAX_COUNT + 1);
		for (var i = 0; i < count; i++)
		{
			yield return CreateRectangle(random);
		}
	}

	public static ShapeBuilder CreateShape(Random random)
		=> new(CreateRectangles(random).ToImmutableArray());
}
