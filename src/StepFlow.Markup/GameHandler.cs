using System.Diagnostics.Metrics;
using System.Drawing;
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

		public void Init()
		{
			CreateRoom(new(5, 5), new(40, 20), 15);

			PlayMaster.CreateItem.Execute(new() { X = 130, Y = 50, Kind = ItemKind.Fire });
			PlayMaster.CreateItem.Execute(new() { X = 200, Y = 50, Kind = ItemKind.Poison });
			PlayMaster.CreateItem.Execute(new() { X = 270, Y = 50, Kind = ItemKind.Speed });
			PlayMaster.CreateItem.Execute(new() { X = 340, Y = 50, Kind = ItemKind.AttackSpeed });
			PlayMaster.CreateItem.Execute(new() { X = 410, Y = 50, Kind = ItemKind.AddStrength });

			PlayMaster.CreatePlace.Execute(new() { Bounds = new(400, 150, 50, 50) });

			CreateEnemy(new(500, 180, 20, 20), 50);
			CreateEnemy(new(200, 180, 20, 20), 50);

			CreateObstruction(new(50, 100, 40, 40), 150);

			CreatePlayerCharacter(new(100, 100, 20, 20), 1000);
		}

		private void CreatePlayerCharacter(Rectangle bounds, int strength)
		{
			PlayMaster.PlayerCharacterCreate.Execute(new()
			{
				X = bounds.X,
				Y = bounds.Y,
				Width = bounds.Width,
				Height = bounds.Height,
				Strength = strength,
			});
		}

		private void CreateRoom(Point location, Size size, int width)
		{
			var top = new Rectangle[size.Width];
			var bottom = new Rectangle[size.Width];
			for (var iX = 0; iX < size.Width; iX++)
			{
				top[iX] = CreatePixel(new(iX, 0));
				bottom[iX] = CreatePixel(new(iX, size.Height));
			}

			var left = new Rectangle[size.Height];
			var right = new Rectangle[size.Height];
			for (var iY = 0; iY < size.Height; iY++)
			{
				left[iY] = CreatePixel(new(0, iY));
				right[iY] = CreatePixel(new(size.Width, iY));
			}

			PlayMaster.CreateObstruction.Execute(new() { Bounds = top, Kind = ObstructionKind.Tiles, });
			PlayMaster.CreateObstruction.Execute(new() { Bounds = bottom, Kind = ObstructionKind.Tiles, });
			PlayMaster.CreateObstruction.Execute(new() { Bounds = left, Kind = ObstructionKind.Tiles, });
			PlayMaster.CreateObstruction.Execute(new() { Bounds = right, Kind = ObstructionKind.Tiles, });

			Rectangle CreatePixel(Point position) => new Rectangle(
				location.X + position.X * width,
				location.Y + position.Y * width,
				width,
				width
			);
		}

		private void CreateObstruction(Rectangle bounds, int? strength)
		{
			PlayMaster.CreateObstruction.Execute(new()
			{
				Bounds = new[] { bounds },
				Strength = strength,
				Kind = ObstructionKind.Single,
			});
		}

		private void PlayerCharacterSetCourse(Course? course)
		{
			PlayMaster.PlayerCharacterSetCourse.Execute(new() { Course = course, });
		}

		private void CreateProjectile(Course course)
		{
			PlayMaster.CreateProjectile.Execute(new() { Course = course, });
		}

		private void CreateEnemy(Rectangle bounds, int visionSize)
		{
			var vision = Rectangle.FromLTRB(
				bounds.Left - visionSize,
				bounds.Top - visionSize,
				bounds.Right + visionSize,
				bounds.Bottom + visionSize
			);

			PlayMaster.CreateEnemy.Execute(new()
			{
				X = bounds.X,
				Y = bounds.Y,
				Width = bounds.Width,
				Height = bounds.Height,
				VisionX = vision.X,
				VisionY = vision.Y,
				VisionWidth = vision.Width,
				VisionHeight = vision.Height,
				ReleaseItem = ItemKind.AddStrength,
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

			foreach (var place in playground.Places)
			{
				CreateTexture(place.Body?.Current.Bounds ?? Rectangle.Empty, Texture.PoisonPlace, null);
			}

			CreateTexture(playground.PlayerCharacter?.Body?.Current.Bounds ?? Rectangle.Empty, Texture.Character, playground.PlayerCharacter?.Strength);
			CreateBorder(playground.PlayerCharacter?.Body, Color.Red);

			foreach (var barrier in playground.Obstructions)
			{
				switch (barrier.Kind)
				{
					case ObstructionKind.Single:
						CreateTexture(barrier.Body?.Current.Bounds ?? Rectangle.Empty, Texture.Wall, barrier.Strength);
						break;
					case ObstructionKind.Tiles:
						foreach (var bounds in barrier.Body?.Current ?? Enumerable.Empty<Rectangle>())
						{
							CreateTexture(bounds, Texture.Wall, barrier.Strength);
						}
						break;
					default: throw EnumNotSupportedException.Create<ObstructionKind>(barrier.Kind);
				}
			}

			foreach (var projectile in playground.Projectiles)
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

			foreach (var item in playground.Items)
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

			foreach (var enemy in playground.Enemies)
			{
				CreateTexture(enemy.Body?.Current.Bounds ?? Rectangle.Empty, Texture.Enemy, enemy.Strength);
				CreateBorder(enemy.Body, Color.Red);
				CreateBorder(enemy.Vision, Color.Yellow);
			}
		}

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