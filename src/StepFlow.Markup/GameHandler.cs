using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Markup.Services;
using StepFlow.Master;

namespace StepFlow.Markup
{
	public sealed class GameHandler
	{
		public GameHandler(
			RectangleF placeBounds,
			IDrawer drawer,
			IKeyboard keyboard
		)
		{
			Drawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
			Keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
			Place = placeBounds;

			Meter.CreateObservableGauge("Time", () => PlayMaster.TimeAxis.Current);
			Meter.CreateObservableGauge("Commands", () => PlayMaster.TimeAxis.Source.Current);
			Meter.CreateObservableGauge("Frame", () => Frame.TotalMilliseconds);
			Meter.CreateObservableGauge("Shapes", () => Playground.IntersectionContext.Count);
			Init();
		}

		private Meter Meter { get; } = new Meter("Game.Gameplay");

		private TimeSpan Frame { get; set; }

		private PlayMaster PlayMaster { get; } = new PlayMaster();

		private IKeyboard Keyboard { get; }

		public IDrawer Drawer { get; }

		public RectangleF Place { get; }

		private static Size CellSize => new(Playground.CELL_SIZE_DEFAULT, Playground.CELL_SIZE_DEFAULT);

		private static int PlaygroundToGlobal(int value) => (value + 1) * Playground.CELL_SIZE_DEFAULT;

		private static Point PlaygroundToGlobal(int x, int y) => new(PlaygroundToGlobal(x), PlaygroundToGlobal(y));

		private static Point PlaygroundToGlobal(Point position) => PlaygroundToGlobal(position.X, position.Y);

		private static Rectangle CreateCell(int x, int y) => new(PlaygroundToGlobal(x, y), CellSize);

		private static Rectangle CreateCell(Point position) => CreateCell(position.X, position.Y);

		public void Init()
		{
			CreateRoom(Point.Empty, new(15, 9), Playground.CELL_SIZE_DEFAULT);

			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(1, 0), Kind = ItemKind.Fire });
			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(2, 0), Kind = ItemKind.Poison });
			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(3, 0), Kind = ItemKind.Speed });
			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(4, 0), Kind = ItemKind.AttackSpeed });

			//PlayMaster.CreatePlace.Execute(new() { Bounds = new(400, 150, 50, 50) });

			CreateEnemy(CreateCell(6, 0), 300, Strategy.Reflection, new(2, 1));
			CreateEnemy(CreateCell(6, 1), 150, Strategy.CW, new(1, 0));

			CreateCellObstruction(new Point(5, 0), 150, ObstructionView.Bricks);
			CreateCellObstruction(new Point(5, 1), 150, ObstructionView.Bricks);
			CreateCellObstruction(new Point(5, 2), 150, ObstructionView.Bricks);
			CreateCellObstruction(new Point(5, 3), 150, ObstructionView.Boards);

			PlayMaster.PlayerCharacterCreate.Execute(new()
			{
				Bounds = CreateCell(0, 0),
				Strength = 1000,
				Speed = 60,
				Cooldown = TimeTick.FromSeconds(1),
			});
		}

		private void CreateRoom(Point location, Size size, int width)
		{
			var top = new Rectangle[size.Width + 1];
			var bottom = new Rectangle[size.Width + 1];
			for (var iX = 0; iX <= size.Width; iX++)
			{
				top[iX] = CreatePixel(new(iX, 0));
				bottom[iX] = CreatePixel(new(iX, size.Height));
			}

			var left = new Rectangle[size.Height + 1];
			var right = new Rectangle[size.Height + 1];
			for (var iY = 0; iY <= size.Height; iY++)
			{
				left[iY] = CreatePixel(new(0, iY));
				right[iY] = CreatePixel(new(size.Width, iY));
			}

			PlayMaster.CreateObstruction.Execute(new() { Bounds = top, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, });
			PlayMaster.CreateObstruction.Execute(new() { Bounds = bottom, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, });
			PlayMaster.CreateObstruction.Execute(new() { Bounds = left, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, });
			PlayMaster.CreateObstruction.Execute(new() { Bounds = right, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, });

			Rectangle CreatePixel(Point position) => new Rectangle(
				location.X + position.X * width,
				location.Y + position.Y * width,
				width,
				width
			);
		}

		private void CreateCellObstruction(Point position, int? strength, ObstructionView view)
		{
			PlayMaster.CreateObstruction.Execute(new()
			{
				Bounds = new[] { CreateCell(position) },
				Strength = strength,
				Kind = ObstructionKind.Single,
				View = view,
			});
		}

		private void PlayerCharacterSetCourse(Course? course)
		{
			PlayMaster.PlayerCharacterSetCourse.Execute(new() { Course = course, });
		}

		private void CreateProjectile(Course course)
		{
			PlayMaster.PlayerCharacterCreateProjectile.Execute(new() { Course = course, });
		}

		private void CreateEnemy(Rectangle bounds, int visionSize, Strategy strategy, Vector2 beginVector)
		{
			PlayMaster.CreateEnemy.Execute(new()
			{
				Bounds = bounds,
				Vision = Rectangle.FromLTRB(
					bounds.Left - visionSize,
					bounds.Top - visionSize,
					bounds.Right + visionSize,
					bounds.Bottom + visionSize
				),
				Strategy = strategy,
				ReleaseItem = ItemKind.AddStrength,
				BeginVector = beginVector
			});
		}

		public void Update()
		{
			if (Keyboard.IsUndo())
			{
				for (var i = 0; i < TimeTick.TICKS_PER_FRAME; i++)
				{
					PlayMaster.TimeAxis.Revert();
				}
			}
			else
			{
				var sw = System.Diagnostics.Stopwatch.StartNew();
				for (var i = 0; i < TimeTick.TICKS_PER_FRAME; i++)
				{
					var transaction = PlayMaster.TimeAxis.BeginTransaction();

					PlayerCharacterSetCourse(Keyboard.GetPlayerCourse());

					if (Keyboard.GetPlayerShot() is { } playerShot)
					{
						CreateProjectile(playerShot);
					}

					PlayMaster.TakeStep.Execute(null);

					transaction.Commit();
				}

				Frame = sw.Elapsed;
			}
		}

		public void Draw()
		{
			var floorTileSize = new Size(40, 40);
			for (var iX = 0; iX < Place.Width; iX += floorTileSize.Width)
			{
				for (var iY = 0; iY < Place.Height; iY += floorTileSize.Height)
				{
					Drawer.Draw(Texture.Floor, new(new Point(iX, iY), floorTileSize));
				}
			}

			var playground = PlayMaster.Playground;

			foreach (var place in playground.Items.OfType<Place>())
			{
				CreateTexture(place.Body?.Current.Bounds ?? Rectangle.Empty, Texture.PoisonPlace, null);
			}

			var playerCharacter = playground.GetPlayerCharacterRequired();
			CreateTexture(playerCharacter?.Body?.Current.Bounds ?? Rectangle.Empty, Texture.Character, playerCharacter?.Strength);
			CreateBorder(playerCharacter?.Body, Color.Red);

			foreach (var barrier in playground.Items.OfType<Obstruction>())
			{
				var textureView = ToTexture(barrier.View);
				switch (barrier.Kind)
				{
					case ObstructionKind.Single:
						CreateTexture(barrier.Body?.Current.Bounds ?? Rectangle.Empty, textureView, barrier.Strength);
						break;
					case ObstructionKind.Tiles:
						foreach (var bounds in barrier.Body?.Current ?? Enumerable.Empty<Rectangle>())
						{
							CreateTexture(bounds, textureView, barrier.Strength);
						}
						break;
					default: throw EnumNotSupportedException.Create<ObstructionKind>(barrier.Kind);
				}
			}

			foreach (var projectile in playground.Items.OfType<Projectile>())
			{
				var textureName = projectile.Damage.Kind switch
				{
					DamageKind.None => Texture.Projectile,
					DamageKind.Fire => Texture.ProjectileFire,
					DamageKind.Poison => Texture.ProjectilePoison,
					DamageKind.Fire | DamageKind.Poison => Texture.ProjectileAll,
					_ => Texture.Projectile,
				};

				CreateTexture(projectile.Body?.Current.Bounds ?? Rectangle.Empty, textureName, null);
			}

			foreach (var item in playground.Items.OfType<Item>())
			{
				var textureName = item.Kind switch
				{
					ItemKind.Fire => Texture.ItemFire,
					ItemKind.Poison => Texture.ItemPoison,
					ItemKind.Speed => Texture.ItemSpeed,
					ItemKind.AttackSpeed => Texture.ItemAttackSpeed,
					ItemKind.AddStrength => Texture.ItemAddStrength,
					_ => Texture.ItemUnknown,
				};

				CreateTexture(item.Body?.Current.Bounds ?? Rectangle.Empty, textureName, null);
			}

			foreach (var enemy in playground.Items.OfType<Enemy>())
			{
				CreateTexture(enemy.Body?.Current.Bounds ?? Rectangle.Empty, Texture.Enemy, enemy.Strength);
				CreateBorder(enemy.Body, Color.Red);
				CreateBorder(enemy.Vision, Color.Yellow);
			}
		}

		private static Texture ToTexture(ObstructionView view) => view switch
		{
			ObstructionView.None => Texture.ObstructionNone,
			ObstructionView.DarkWall => Texture.ObstructionDarkWall,
			ObstructionView.Bricks => Texture.ObstructionBricks,
			ObstructionView.Boards => Texture.ObstructionBoards,
			_ => throw EnumNotSupportedException.Create(view),
		};

		private void CreateTexture(Rectangle bounds, Texture texture, Scale? strength)
		{
			if (!bounds.IsEmpty)
			{
				Drawer.Draw(texture, bounds);
				if (strength is not null)
				{
					var strengthBounds = new Rectangle(
						bounds.Left,
						bounds.Top,
						bounds.Width,
						0
					);
					Drawer.DrawString(
						strength.Value.ToString(),
						strengthBounds,
						HorizontalAlign.Center,
						VerticalAlign.Bottom,
						Color.Red
					);
				}
			}
		}

		private void CreateBorder(Collided? collided, Color color)
		{
			if (collided is { })
			{
				var bounds = collided.Current.Bounds;
				Drawer.Polygon(
					new PointF[]
					{
						new(bounds.Left, bounds.Top),
						new(bounds.Right, bounds.Top),
						new(bounds.Right, bounds.Bottom),
						new(bounds.Left, bounds.Bottom),
					},
					color
				);
			}
		}
	}
}