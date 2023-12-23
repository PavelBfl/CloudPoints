using System.Diagnostics.Metrics;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Markup.Services;
using StepFlow.Master;
using StepFlow.Master.Proxies.Components;

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

			Meter.CreateObservableGauge("Time", () => PlayMaster.Time);
			Meter.CreateObservableGauge("Commands", () => PlayMaster.TimeAxis.Count);
			Meter.CreateObservableGauge("Frame", () => Frame.TotalMilliseconds);
			Init();
		}

		private Meter Meter { get; } = new Meter("Game.Gameplay");

		private TimeSpan Frame { get; set; }

		private PlayMaster PlayMaster { get; } = new PlayMaster();

		private IKeyboard Keyboard { get; }

		private IDrawer Drawer { get; }

		public RectangleF Place { get; }

		public void Init()
		{
			CreateRoom(new(5, 5), new(40, 20), 15);

			CreateDamageItem(new(130, 50, 15, 15), 10, DamageKind.Fire);
			CreateDamageItem(new(200, 50, 15, 15), 10, DamageKind.Poison);
			CreateSpeedItem(new(270, 50, 15, 15), 5);

			CreateEnemy(new(50, 180, 20, 20), 50);
			CreateEnemy(new(200, 180, 20, 20), 50);

			CreateObstruction(new(50, 100, 40, 40), 150);

			CreatePlayerCharacter(new(100, 100, 20, 20), 100);
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
			for (var iX = 0; iX < size.Width; iX++)
			{
				CreateBounds(new(iX, 0));
			}

			for (var iY = 0; iY < size.Height; iY++)
			{
				CreateBounds(new(size.Width, iY));
			}

			for (var iX = size.Width; iX > 0; iX--)
			{
				CreateBounds(new(iX, size.Height));
			}

			for (var iY = size.Height; iY > 0; iY--)
			{
				CreateBounds(new(0, iY));
			}

			void CreateBounds(Point index)
			{
				var tileLocation = new Point(index.X * width + location.X, index.Y * width + location.Y);
				var bounds = new Rectangle(tileLocation.X, tileLocation.Y, width, width);
				CreateObstruction(bounds, null);
			}
		}

		private void CreateObstruction(Rectangle bounds, int? strength)
		{
			PlayMaster.CreateObstruction.Execute(new()
			{
				X = bounds.X,
				Y = bounds.Y,
				Width = bounds.Width,
				Height = bounds.Height,
				Strength = strength,
			});
		}

		private void PlayerCharacterSetCourse(Course course)
		{
			if (PlayMaster.Playground.PlayerCharacter is { CurrentAction: null })
			{
				PlayMaster.PlayerCharacterSetCourse.Execute(new() { Course = course, });
			}
		}

		private void CreateProjectile(Course course)
		{
			PlayMaster.CreateProjectile.Execute(new() { Course = course, });
		}

		private void CreateDamageItem(Rectangle bounds, int value, DamageKind kind)
		{
			PlayMaster.CreateDamageItem.Execute(new()
			{
				X = bounds.X,
				Y = bounds.Y,
				Width = bounds.Width,
				Height = bounds.Height,
				Value = value,
				Kind = kind
			});
		}

		private void CreateSpeedItem(Rectangle bounds, int speed)
		{
			PlayMaster.CreateSpeedItem.Execute(new()
			{
				X = bounds.X,
				Y = bounds.Y,
				Width = bounds.Width,
				Height = bounds.Height,
				Speed = speed,
			});
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
			});
		}

		public void Update()
		{
			const int TICKS_COUNT = 100;
			if (Keyboard.IsUndo())
			{
				for (var i = 0; i < TICKS_COUNT; i++)
				{
					PlayMaster.TimeAxis.Revert();
				}
			}
			else
			{
				var sw = System.Diagnostics.Stopwatch.StartNew();
				for (var i = 0; i < TICKS_COUNT; i++)
				{
					if (Keyboard.GetPlayerCourse() is { } playerCourse)
					{
						PlayerCharacterSetCourse(playerCourse);
					}

					if (Keyboard.GetPlayerShot() is { } playerShot)
					{
						CreateProjectile(playerShot);
					}

					PlayMaster.TakeStep.Execute(null);
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

			var playground = PlayMaster.GetPlaygroundProxy();

			CreateTexture(playground.PlayerCharacter?.Body, Texture.Character, null);
			CreateBorder(playground.PlayerCharacter?.Body, Color.Red);

			foreach (var barrier in playground.Obstructions)
			{
				CreateTexture(barrier.Body, Texture.Wall, barrier.Strength);
			}

			foreach (var projectile in playground.Projectiles)
			{
				var textureName = projectile.Damage?.Kind switch
				{
					DamageKind.None => Texture.Projectile,
					DamageKind.Fire => Texture.ProjectileFire,
					DamageKind.Poison => Texture.ProjectilePoison,
					DamageKind.Fire | DamageKind.Poison => Texture.ProjectileAll,
					_ => Texture.Projectile,
				};

				CreateTexture(projectile.Body, textureName, null);
			}

			foreach (var item in playground.Items)
			{
				var textureName = item.Kind switch
				{
					ItemKind.Fire => Texture.ItemFire,
					ItemKind.Poison => Texture.ItemPoison,
					ItemKind.Speed => Texture.ItemSpeed,
					_ => Texture.ItemUnknown,
				};

				CreateTexture(item.Body, textureName, null);
			}

			foreach (var enemy in playground.Enemies)
			{
				CreateTexture(enemy.Body, Texture.Enemy, null);
				CreateBorder(enemy.Body, Color.Red);
				CreateBorder(enemy.Vision, Color.Yellow);
			}
		}

		private void CreateTexture(ICollidedProxy? collided, Texture texture, IScaleProxy? strength)
		{
			if (collided?.Target is { Current: { } current })
			{
				Drawer.Draw(texture, current.Value.Bounds);
				if (strength is not null)
				{
					Drawer.DrawString(
						strength.Value.ToString(),
						current.Value.Bounds,
						HorizontalAlign.Center,
						VerticalAlign.Center,
						Color.Red
					);
				}
			}
		}

		private void CreateBorder(ICollidedProxy? collided, Color color)
		{
			if (collided?.Target is { Current: { } current })
			{
				var bounds = current.Value.Bounds;
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