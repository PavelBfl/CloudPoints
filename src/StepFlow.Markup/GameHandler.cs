using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Intersection;
using StepFlow.Markup.Services;
using StepFlow.Master;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Markup
{
	public sealed class GameHandler
	{
		public GameHandler(
			RectangleF placeBounds,
			IDrawer drawer,
			IControl control
		)
		{
			Drawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
			Control = control ?? throw new ArgumentNullException(nameof(control));
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

		private IControl Control { get; }

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
			CreateRoom(Point.Empty, new(14, 8), Playground.CELL_SIZE_DEFAULT);

			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(12, 6), Kind = ItemKind.Fire });
			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(12, 0), Kind = ItemKind.Poison });
			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(1, 1), Kind = ItemKind.Speed });
			PlayMaster.CreateItem.Execute(new() { Position = PlaygroundToGlobal(1, 5), Kind = ItemKind.AttackSpeed });

			PlayMaster.CreatePlace.Execute(new() { Bounds = CreateCell(8, 0) });
			PlayMaster.CreatePlace.Execute(new() { Bounds = CreateCell(8, 1) });
			PlayMaster.CreatePlace.Execute(new() { Bounds = CreateCell(10, 1) });
			PlayMaster.CreatePlace.Execute(new() { Bounds = CreateCell(10, 2) });
			PlayMaster.CreatePlace.Execute(new() { Bounds = CreateCell(11, 2) });
			PlayMaster.CreatePlace.Execute(new() { Bounds = CreateCell(12, 2) });

			CreateEnemy(CreateCell(10, 6), 300, Strategy.Reflection, new(2, 1));
			CreateEnemy(CreateCell(0, 0), 150, Strategy.CW, new(1, 0));

			CreateCellObstruction(new Point(3, 0), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(3, 1), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(3, 2), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(3, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(3, 4), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(3, 5), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(3, 6), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(0, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(1, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(2, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(11, 0), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(12, 1), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(8, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(9, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(10, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(11, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(12, 3), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(12, 5), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(11, 6), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(6, 4), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(7, 4), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(8, 4), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(6, 5), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(5, 6), 50, ObstructionView.Bricks);
			CreateCellObstruction(new Point(6, 6), 50, ObstructionView.Bricks);

			CreateCellObstruction(new Point(0, 4), 50, ObstructionView.Boards, 1);
			CreateCellObstruction(new Point(1, 4), 50, ObstructionView.Boards, 1);
			CreateCellObstruction(new Point(2, 4), 50, ObstructionView.Boards, 1);
			CreateCellObstruction(new Point(0, 5), 50, ObstructionView.Boards, 1);
			CreateCellObstruction(new Point(2, 5), 50, ObstructionView.Boards, 1);
			CreateCellObstruction(new Point(0, 6), 50, ObstructionView.Boards, 1);
			CreateCellObstruction(new Point(1, 6), 50, ObstructionView.Boards, 1);
			CreateCellObstruction(new Point(2, 6), 50, ObstructionView.Boards, 1);

			PlayMaster.PlayerCharacterCreate.Execute(new()
			{
				Bounds = CreateCell(5, 0),
				Strength = 1000,
				Speed = 3,
				Cooldown = TimeTick.FromSeconds(1),
			});
			CreateCellObstruction(new Point(5, 1), 1000, ObstructionView.Boards, 1);
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

		private void CreateCellObstruction(Point position, int? strength, ObstructionView view, int weight = Material.MAX_WEIGHT)
		{
			PlayMaster.CreateObstruction.Execute(new()
			{
				Bounds = new[] { CreateCell(position) },
				Strength = strength,
				Kind = ObstructionKind.Single,
				View = view,
				Weight = weight,
			});
		}

		private void PlayerCharacterSetCourse(float? course)
		{
			PlayMaster.PlayerCharacterSetCourse.Execute(new() { Course = course, });
		}

		private void CreateProjectile(float radians, PlayerAction action)
		{
			PlayMaster.PlayerCharacterCreateProjectile.Execute(new() { Radians = radians, Action = action });
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

		private const int TIME_TICKS_STEP = 10;

		private int currentTicks = TimeTick.TICKS_PER_FRAME;

		private int CurrentTicks
		{
			get => currentTicks;
			set
			{
				currentTicks = value;

				if (currentTicks < 0)
				{
					currentTicks = 0;
				}
				else if (currentTicks > TimeTick.TICKS_PER_FRAME)
				{
					currentTicks = TimeTick.TICKS_PER_FRAME;
				}
			}
		}

		public void Update()
		{
			if (Control.OnSwitchDebug())
			{
				IsDebug = !IsDebug;
			}

			var timeOffset = Control.GetTimeOffset();
			switch (timeOffset)
			{
				case TimeOffset.None:
					break;
				case TimeOffset.Up:
					CurrentTicks += TIME_TICKS_STEP;
					break;
				case TimeOffset.Down:
					CurrentTicks -= TIME_TICKS_STEP;
					break;
				default: throw EnumNotSupportedException.Create(timeOffset);
			}

			if (Control.IsUndo())
			{
				for (var i = 0; i < TimeTick.TICKS_PER_FRAME * 5; i++)
				{
					PlayMaster.TimeAxis.Revert();
				}
			}
			else
			{
				var sw = System.Diagnostics.Stopwatch.StartNew();
				for (var i = 0; i < CurrentTicks; i++)
				{
					var transaction = PlayMaster.TimeAxis.BeginTransaction();

					PlayerCharacterSetCourse(Control.GetPlayerCourse());

					var playerAction = Control.GetPlayerAction();
					switch (playerAction)
					{
						case PlayerAction.None:
							break;
						case PlayerAction.Main:
						case PlayerAction.Auxiliary:
							var playerCharacter = PlayMaster.Playground.GetPlayerCharacterRequired();
							var center = playerCharacter.GetBodyRequired().Current.Bounds.GetCenter();
							CreateProjectile(Control.GetPlayerRotate(new(center.X, center.Y)), playerAction);
							break;
						default: throw EnumNotSupportedException.Create(playerAction);
					};

					PlayMaster.TakeStep.Execute(null);

					foreach (var material in PlayMaster.Playground.Items.OfType<Material>())
					{
						var trackProxy = (ITrackProxy?)PlayMaster.CreateProxy(material.Track);
						trackProxy?.Offset(material.Body.Current.Bounds);
					}

					transaction.Commit();
				}

				Frame = sw.Elapsed;
			}
		}

		private bool IsDebug { get; set; } = false;

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
				var textureView = ToTexture(barrier.View, barrier.Strength);
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

				foreach (var step in projectile.Track?.Steps ?? Enumerable.Empty<RectangleF>())
				{
					CreateTexture(
						new Rectangle((int)step.X, (int)step.Y, (int)step.Width, (int)step.Height),
						textureName,
						null
					);
				}

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

		private static Texture ToTexture(ObstructionView view, Scale? scale) => view switch
		{
			ObstructionView.None => Texture.ObstructionNone,
			ObstructionView.DarkWall => Texture.ObstructionDarkWall,
			ObstructionView.Bricks => ToPct(scale) > 0.5f ? Texture.ObstructionBricks : Texture.ObstructionBricksDamaged,
			ObstructionView.Boards => Texture.ObstructionBoards,
			_ => throw EnumNotSupportedException.Create(view),
		};

		private static float ToPct(Scale? scale) => scale is not null ? (float)scale.Value / scale.Max : 0;

		private void CreateTexture(Rectangle bounds, Texture texture, Scale? strength)
		{
			if (!bounds.IsEmpty)
			{
				Drawer.Draw(texture, bounds);
				if (strength is not null && IsDebug)
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
			if (collided is { } && IsDebug)
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