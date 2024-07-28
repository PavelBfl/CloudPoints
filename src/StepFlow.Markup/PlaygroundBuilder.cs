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

	private static ObstructionDto CreateBricks(int x, int y) => new ObstructionDto()
	{
		Kind = ObstructionKind.Single,
		View = ObstructionView.Bricks,
		Strength = Scale.CreateByMax(50),
		Weight = Material.MAX_WEIGHT,
		Body = new CollidedDto()
		{
			Current = { CreateCell(x, y), },
			IsRigid = true,
		},
	};

	private static ObstructionDto CreateBoards(int x, int y) => new ObstructionDto()
	{
		Kind = ObstructionKind.Single,
		View = ObstructionView.Boards,
		Strength = Scale.CreateByMax(50),
		Weight = 1,
		Body = new CollidedDto()
		{
			Current = { CreateCell(x, y), },
			IsRigid = true,
		},
	};

	private static ItemDto CreateItem(int x, int y, ItemKind kind) => new ItemDto()
	{
		Kind = kind,
		Body = new CollidedDto()
		{
			Current = { CreateCell(x, y), },
			IsRigid = true,
		},
	};

	private static EnemyDto CreateEnemy(int x, int y, int visionSize, Vector2 course, CollisionBehavior collisionBehavior)
	{
		var bounds = CreateCell(x, y);
		return new()
		{
			Body = new CollidedDto()
			{
				Current = { bounds, },
				IsRigid = true,
			},
			Vision = new CollidedDto()
			{
				Current =
				{
					Rectangle.FromLTRB(
						bounds.Left - visionSize,
						bounds.Top - visionSize,
						bounds.Right + visionSize,
						bounds.Bottom + visionSize
					),
				},
			},
			ReleaseItem = ItemKind.AddStrength,
			Course = course,
			CollisionBehavior = collisionBehavior,
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
				CreateBricks(4, 1),
				CreateBricks(4, 2),
				CreateBricks(4, 3),
				CreateBricks(4, 4),
				CreateBricks(4, 5),
				CreateBricks(4, 6),
				CreateBricks(4, 7),
				CreateBricks(1, 4),
				CreateBricks(2, 4),
				CreateBricks(3, 4),
				CreateBricks(12, 1),
				CreateBricks(13, 2),
				CreateBricks(9, 4),
				CreateBricks(10, 4),
				CreateBricks(11, 4),
				CreateBricks(12, 4),
				CreateBricks(13, 4),
				CreateBricks(13, 6),
				CreateBricks(12, 7),
				CreateBricks(7, 5),
				CreateBricks(8, 5),
				CreateBricks(9, 5),
				CreateBricks(7, 6),
				CreateBricks(6, 7),
				CreateBricks(7, 7),
				CreateBoards(1, 5),
				CreateBoards(2, 5),
				CreateBoards(3, 5),
				CreateBoards(1, 6),
				CreateBoards(3, 6),
				CreateBoards(1, 7),
				CreateBoards(2, 7),
				CreateBoards(3, 7),
				CreateItem(13, 7, ItemKind.Fire),
				CreateItem(13, 1, ItemKind.Poison),
				CreateItem(2, 2, ItemKind.Speed),
				CreateItem(2, 6, ItemKind.AttackSpeed),
				//CreateEnemy(11, 7, 300, CreateRotate(MathF.PI / 4) * 0.02f, CollisionBehavior.Reflection),
				//CreateEnemy(1, 1, 150, CreateRotate(0) * 0.02f, CollisionBehavior.CW),
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
