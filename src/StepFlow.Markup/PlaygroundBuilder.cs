using System.Drawing;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Domains;
using StepFlow.Domains.Components;
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
			Strength = Scale.CreateByMax(50),
			ReleaseItem = ItemKind.AddStrength,
			Course = course,
			CollisionBehavior = collisionBehavior,
			Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(1)),
		};
	}

	private static EnemyDto CreateBoss(int x, int y, Vector2 course)
	{
		return new()
		{
			Body = new CollidedDto()
			{
				Current = { CreateCell(x, y) },
				IsRigid = true,
			},
			Strength = Scale.CreateByMax(50),
			Course = course,
			CollisionBehavior = CollisionBehavior.Reflection,
			Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(1)),
			States =
			{
				new()
				{
					Kind = StateKind.EnemySerpentineForwardState,
					Enable = true,
					Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(4)),
					Arg0 = 0,
					Arg1 = 0.02f,
				},
				new()
				{
					Kind = StateKind.EnemySerpentineForwardStateAttack,
					Enable = true,
					Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(1)),
					Arg0 = 0,
				},
				new()
				{
					Kind = StateKind.EnemySerpentineForwardToBackward,
					Arg0 = 0.04f,
					Arg1 = 0,
				},
				new()
				{
					Kind = StateKind.EnemySerpentineBackwardState,
					Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(3)),
					Arg0 = 0,
					Arg1 = 0.02f,
				},
				new()
				{
					Kind = StateKind.EnemySerpentineBackwardStateAttack,
					Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(1)),
					Arg0 = MathF.PI,
				},
				new()
				{
					Kind = StateKind.EnemySerpentineBackwardToForward,
					Arg0 = -0.04f,
					Arg1 = 0,
				},
			},
		};
	}

	private static WormholeDto CreateWormhole(Point position, Point positionDestination, Horizontal horizontal, Vertical vertical, string destination) => new WormholeDto()
	{
		Body = new CollidedDto()
		{
			Current = { CreateCell(position) },
		},
		Destination = destination,
		Position = (Vector2)(PointF)GetPosition(CreateCell(positionDestination), horizontal, vertical),
		Horizontal = horizontal,
		Vertical = vertical,
	};

	// TODO Temp
	private static Point GetPosition(Rectangle rectangle, Horizontal horizontal, Vertical vertical)
	{
		return new Point(
			horizontal switch
			{
				Horizontal.Left => rectangle.Left,
				Horizontal.Center => rectangle.X + rectangle.Width / 2,
				Horizontal.Right => rectangle.Right,
				_ => throw EnumNotSupportedException.Create(horizontal),
			},
			vertical switch
			{
				Vertical.Top => rectangle.Top,
				Vertical.Center => rectangle.Y + rectangle.Height / 2,
				Vertical.Bottom => rectangle.Bottom,
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
			yield return CreateWormhole(new(0, 4), new(13, 4), Horizontal.Right, Vertical.Center, left);
			yield return CreateWall(new(0, 5), new(0, 7));
		}

		if (top is null)
		{
			yield return CreateWall(new(0, 0), new(14, 0));
		}
		else
		{
			yield return CreateWall(new(0, 0), new(6, 0));
			yield return CreateWormhole(new(7, 0), new(7, 7), Horizontal.Center, Vertical.Bottom, top);
			yield return CreateWall(new(8, 0), new(14, 0));
		}

		if (right is null)
		{
			yield return CreateWall(new(14, 1), new(14, 7));
		}
		else
		{
			yield return CreateWall(new(14, 1), new(14, 3));
			yield return CreateWormhole(new(14, 4), new(1, 4), Horizontal.Left, Vertical.Center, right);
			yield return CreateWall(new(14, 5), new(14, 7));
		}

		if (bottom is null)
		{
			yield return CreateWall(new(0, 8), new(14, 8));
		}
		else
		{
			yield return CreateWall(new(0, 8), new(6, 8));
			yield return CreateWormhole(new(7, 8), new(7, 1), Horizontal.Center, Vertical.Top, bottom);
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
				CreateEnemy(11, 7, 300, CreateRotate(MathF.PI / 4) * 0.02f, CollisionBehavior.Reflection),
				CreateEnemy(1, 1, 150, CreateRotate(0) * 0.02f, CollisionBehavior.CW),
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
			},
		};
	}

	public PlayerCharacterDto CreatePlayerCharacter0() => new()
	{
		Name = "Player",
		Strength = Scale.CreateByMax(100000),
		Cooldown = Scale.CreateByMin(TimeTick.FromSeconds(1)),
		Speed = 1,
		Body = new CollidedDto()
		{
			Current = { CreateCell(6, 1, 7) },
			IsRigid = true,
		},
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
