using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using System.Drawing;
using System.Numerics;

namespace StepFlow.Markup;

internal class PlaygroundBuilder
{
	private static Size CellSize => new(Playground.CELL_SIZE_DEFAULT, Playground.CELL_SIZE_DEFAULT);

	private static int PlaygroundToGlobal(int value) => value * Playground.CELL_SIZE_DEFAULT;

	private static Point PlaygroundToGlobal(int x, int y) => new(PlaygroundToGlobal(x), PlaygroundToGlobal(y));

	private static Point PlaygroundToGlobal(Point position) => PlaygroundToGlobal(position.X, position.Y);

	private static Rectangle CreateCell(int x, int y) => new(PlaygroundToGlobal(x, y), CellSize);

	private static Rectangle CreateCell(Point position) => CreateCell(position.X, position.Y);

	private static Vector2 CreateRotate(float radians)
	{
		return Vector2.Transform(
			new Vector2(1, 0),
			Matrix3x2.CreateRotation(radians)
		);
	}

	private static ObstructionDto CreateWall(Point p0, Point p1)
	{
		var xMin = Math.Min(p0.X, p1.X);
		var xMax = Math.Max(p0.X, p1.X);
		var yMin = Math.Min(p0.Y, p1.Y);
		var yMax = Math.Max(p0.Y, p1.Y);

		var tiles = new List<Rectangle>();
		for (var iX = xMin; iX <= xMax; iX++)
		{
			for (var iY = yMin; iY <= yMax; iY++)
			{
				tiles.Add(CreateCell(iX, iY));
			}
		}

		return new ObstructionDto()
		{
			Kind = ObstructionKind.Tiles,
			View = ObstructionView.DarkWall,
			Weight = Material.MAX_WEIGHT,
			Body = new CollidedDto()
			{
				Current = { tiles },
				IsRigid = true,
			},
		};
	}

	public PlaygroundDto CreateState0()
	{
		return new PlaygroundDto()
		{
			Items =
			{
				CreateWall(new Point(0, 0), new Point(14, 0)),
				CreateWall(new Point(0, 8), new Point(14, 8)),
				CreateWall(new Point(0, 1), new Point(0, 7)),
				CreateWall(new Point(14, 1), new Point(14, 7)),
				new ObstructionDto()
				{
					Kind = ObstructionKind.Single,
					View = ObstructionView.Bricks,
					Strength = Scale.CreateByMax(50),
					Weight = Material.MAX_WEIGHT,
					Body = new CollidedDto()
					{
						Current = { CreateCell(1, 1), },
						IsRigid= true,
					},
				},
			}
		};
	}
}

internal static class CollectionExtensions
{
	public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
	{
		foreach (var item in items)
		{
			collection.Add(item);
		}
	}
}
