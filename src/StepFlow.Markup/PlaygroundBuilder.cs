using System.Drawing;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Domains;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Master;

namespace StepFlow.Markup;

internal class PlaygroundBuilder
{
	private static Size CellSize => new(Playground.CELL_SIZE_DEFAULT, Playground.CELL_SIZE_DEFAULT);

	private static int PlaygroundToGlobal(int value) => value * Playground.CELL_SIZE_DEFAULT;

	private static Point PlaygroundToGlobal(int x, int y) => new(PlaygroundToGlobal(x), PlaygroundToGlobal(y));

	private static Point PlaygroundToGlobal(Point position) => PlaygroundToGlobal(position.X, position.Y);

	private static Rectangle CreateCell(int x, int y, int margin = 0)
	{
		var position = PlaygroundToGlobal(x, y);
		var size = CellSize;
		return new(
			position.X + margin,
			position.Y + margin,
			size.Width - margin * 2,
			size.Height - margin * 2
		);
	}

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
			Elasticity = 0.1f,
			IsFixed = true,

			BodyCurrent = { tiles },
			IsRigid = true,
		};
	}

	private static ObstructionDto CreateBricks(int x, int y) => new ObstructionDto()
	{
		Name = ObstructionView.Bricks.ToString(),
		Kind = ObstructionKind.Single,
		View = ObstructionView.Bricks,
		Strength = Scale.CreateByMax(50),
		Weight = Material.MAX_WEIGHT,
		BodyCurrent = { CreateCell(x, y), },
		IsRigid = true,
	};

	private static ObstructionDto CreateBoards(int x, int y) => new ObstructionDto()
	{
		Name = ObstructionView.Boards.ToString(),
		Kind = ObstructionKind.Single,
		View = ObstructionView.Boards,
		Strength = Scale.CreateByMax(50),
		Weight = 100,
		Elasticity = 0.9f,
		BodyCurrent = { CreateCell(x, y), },
		IsRigid = true,
		Course = new Vector2(0, 0),
		States =
		{
			new()
			{
				Kind = StateKind.Gravity,
				Arg1 = 0.00001f,
			},
		},
	};

	private static ItemDto CreateItem(int x, int y, ItemKind kind) => new ItemDto()
	{
		Kind = kind,
		BodyCurrent = { CreateCell(x, y), },
		IsRigid = true,
	};

	private static EnemyDto CreateEnemy(int x, int y, int visionSize, Vector2 course)
	{
		var bounds = CreateCell(x, y);
		return new()
		{
			BodyCurrent = { bounds, },
			IsRigid = true,
			Vision =
			{
				Rectangle.FromLTRB(
					bounds.Left - visionSize,
					bounds.Top - visionSize,
					bounds.Right + visionSize,
					bounds.Bottom + visionSize
				),
			},
			Elasticity = 0,
			PatrolSpeed = 0.02f,
			//IsFixed = true,
			Strength = Scale.CreateByMax(50),
			ReleaseItem = ItemKind.AddStrength,
			Course = course,
			Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(1)),
			States =
			{
				new()
				{
					Kind = StateKind.Gravity,
					Arg1 = 0.00001f,
				}
			}
		};
	}

	private static EnemyDto CreateBoss(int x, int y, Vector2 course)
	{
		return new()
		{
			BodyCurrent = { CreateCell(x, y) },
			IsRigid = true,
			Strength = Scale.CreateByMax(50),
			Course = course,
			Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(1)),
		};
	}

	private static WormholeDto CreateWormhole(Point position, Point positionDestination, HorizontalAlign horizontal, VerticalAlign vertical, string destination) => new WormholeDto()
	{
		BodyCurrent = { CreateCell(position) },
		Destination = destination,
		Position = (Vector2)(PointF)GetPosition(CreateCell(positionDestination), horizontal, vertical),
		Horizontal = horizontal,
		Vertical = vertical,
	};

	// TODO Temp
	private static Point GetPosition(Rectangle rectangle, HorizontalAlign horizontal, VerticalAlign vertical)
	{
		return new Point(
			horizontal switch
			{
				HorizontalAlign.Left => rectangle.Left,
				HorizontalAlign.Center => rectangle.X + rectangle.Width / 2,
				HorizontalAlign.Right => rectangle.Right,
				_ => throw EnumNotSupportedException.Create(horizontal),
			},
			vertical switch
			{
				VerticalAlign.Top => rectangle.Top,
				VerticalAlign.Center => rectangle.Y + rectangle.Height / 2,
				VerticalAlign.Bottom => rectangle.Bottom,
				_ => throw EnumNotSupportedException.Create(vertical),
			}
		);
	}

	private static IEnumerable<MaterialDto> CreateRoom(string? left, string? top, string? right, string? bottom)
	{
		if (left is null)
		{
			yield return CreateWall(new(0, 1), new(0, 7));
		}
		else
		{
			yield return CreateWall(new(0, 1), new(0, 3));
			yield return CreateWormhole(new(0, 4), new(13, 4), HorizontalAlign.Right, VerticalAlign.Center, left);
			yield return CreateWall(new(0, 5), new(0, 7));
		}

		if (top is null)
		{
			yield return CreateWall(new(0, 0), new(14, 0));
		}
		else
		{
			yield return CreateWall(new(0, 0), new(6, 0));
			yield return CreateWormhole(new(7, 0), new(7, 7), HorizontalAlign.Center, VerticalAlign.Bottom, top);
			yield return CreateWall(new(8, 0), new(14, 0));
		}

		if (right is null)
		{
			yield return CreateWall(new(14, 1), new(14, 7));
		}
		else
		{
			yield return CreateWall(new(14, 1), new(14, 3));
			yield return CreateWormhole(new(14, 4), new(1, 4), HorizontalAlign.Left, VerticalAlign.Center, right);
			yield return CreateWall(new(14, 5), new(14, 7));
		}

		if (bottom is null)
		{
			yield return CreateWall(new(0, 8), new(14, 8));
		}
		else
		{
			yield return CreateWall(new(0, 8), new(6, 8));
			yield return CreateWormhole(new(7, 8), new(7, 1), HorizontalAlign.Center, VerticalAlign.Top, bottom);
			yield return CreateWall(new(8, 8), new(14, 8));
		}
	}

	public PlaygroundDto CreateState0()
	{
		return new PlaygroundDto()
		{
			Name = "Default",
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
				CreateEnemy(11, 7, 300, CreateRotate(MathF.PI / 4) * 0.02f),
				CreateEnemy(1, 1, 150, CreateRotate(0) * 0.02f),
			}
		};
	}

	public PlaygroundDto CreateState1()
	{
		return new PlaygroundDto()
		{
			Name = "Boss",
			Items =
			{
				CreateWall(new Point(0, 0), new Point(14, 0)),
				CreateWall(new Point(0, 8), new Point(14, 8)),
				CreateWall(new Point(0, 1), new Point(0, 7)),
				CreateWall(new Point(14, 1), new Point(14, 7)),
				CreateBoss(1, 1, new Vector2(0, 0.02f)),
			},
		};
	}

	public PlaygroundDto CreateState2(string current, string? left, string? top, string? right, string? bottom)
	{
		return new()
		{
			Name = current,
			Items =
			{
				CreateRoom(left, top, right, bottom),
				CreateBoards(5, 5),
				CreateBoards(6, 5),
				CreateBoards(7, 5),
				CreateBoards(5, 6),
				CreateBoards(6, 6),
				CreateBoards(7, 6),
				CreateBoards(5, 7),
				CreateBoards(6, 7),
				CreateBoards(7, 7),

				CreateBricks(11, 4),
				CreateBricks(12, 4),
				CreateBricks(13, 4),

				CreateItem(13, 3, ItemKind.Poison),
				CreateItem(1, 7, ItemKind.Fire),

				CreateEnemy(1, 5, 150, CreateRotate(0) * 0.02f),

				//CreateBoards(1, 7),
				//CreateBoards(1, 6),
				//CreateBoards(2, 7),
				//CreateBoards(3, 7),
				//CreateBoards(4, 7),
				//CreateBoards(4, 6),

				//new ObstructionDto()
				//{
				//	Name = ObstructionView.Boards.ToString(),
				//	Kind = ObstructionKind.Single,
				//	View = ObstructionView.Boards,
				//	Strength = Scale.CreateByMax(50),
				//	Weight = 100,
				//	Elasticity = 0.9f,
				//	BodyCurrent = { CreateCell(2, 2), },
				//	IsRigid = true,
				//	Course = new Vector2(0, 0),
				//	Route = new Domains.Tracks.RouteDto()
				//	{
				//		Path =
				//		{
				//			new Domains.Tracks.Curve()
				//			{
				//				Begin = new Vector2(CellSize.Width * 2, CellSize.Height * 2),
				//				BeginControl = new Vector2(CellSize.Width * 2, CellSize.Height * 4),
				//				EndControl = new Vector2(CellSize.Width * 4, CellSize.Height * 4),
				//				End = new Vector2(CellSize.Width * 4, CellSize.Height * 2),
				//			},
				//		},
				//		Speed = 0.001f,
				//	},
				//}
			},
		};
	}

	public PlayerCharacterDto CreatePlayerCharacter0() => new()
	{
		Name = "Player",
		Strength = Scale.CreateByMax(100000),
		Cooldown = Scale.CreateByMin(TimeTick.FromSeconds(1)),
		Speed = 1,
		BodyCurrent = { CreateCell(8, 1, 7) },
		IsRigid = true,
		Weight = 1,
		Elasticity = 0,
		States =
		{
			new()
			{
				Kind = StateKind.Gravity,
				Arg1 = 0.00001f,
			},
		},
	};
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
