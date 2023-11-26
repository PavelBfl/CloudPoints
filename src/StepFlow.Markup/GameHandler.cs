using System.Diagnostics.Metrics;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;
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
			IKeyboard keyboard,
			IMouse mouse
		)
		{
			Drawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
			Keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
			Mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
			Place = placeBounds;

			Meter.CreateObservableGauge("Time", () => PlayMaster.Time);
			Meter.CreateObservableGauge("Commands", () => PlayMaster.TimeAxis.Count);
			Init();
		}

		private Meter Meter { get; } = new Meter("Game.Gameplay");

		private PlayMaster PlayMaster { get; } = new PlayMaster();

		private IMouse Mouse { get; }

		private IKeyboard Keyboard { get; }

		private IDrawer Drawer { get; }

		public RectangleF Place { get; }

		public void Init()
		{
			CreateRoom(new(5, 5), new(40, 20), 15);

			CreateItem(new(130, 50, 15, 15), 10, DamageKind.Fire);
			CreateItem(new(200, 50, 15, 15), 10, DamageKind.Poison);

			CreateEnemy(new(50, 180, 20, 20), 50);
			CreateEnemy(new(200, 180, 20, 20), 50);

			CreateObstruction(new(50, 50, 40, 40), 150);

			CreatePlayerCharacter(new(100, 100, 20, 20), 100);
		}

		private void CreatePlayerCharacter(Rectangle bounds, float strength)
		{
			PlayMaster.Execute(@$"
				playground.CreatePlayerCharacter(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					{strength}
				);
			");
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

		private void CreateObstruction(Rectangle bounds, float? strength)
		{
			PlayMaster.Execute(@$"
				playground.CreateObstruction(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					{strength?.ToString() ?? "null"}
				);
			");
		}

		private void PlayerCharacterSetCourse(Course course)
		{
			if (PlayMaster.Playground.PlayerCharacter is { } playerCharacter && playerCharacter.Scheduler.Queue.LastOrDefault().Executor is not SetCourse)
			{
				PlayMaster.Execute($@"
					playground.PlayerCharacter.SetCourse({(int)course});
				");
			}
		}

		private void CreateProjectile(Course course)
		{
			PlayMaster.Execute($@"
				playground.PlayerCharacter.CreateProjectile({(int)course});
			");
		}

		private void CreateItem(Rectangle bounds, int value, DamageKind kind)
		{
			PlayMaster.Execute($@"
				playground.CreateItem(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					{value},
					{(int)kind}
				);
			");
		}

		private void CreateEnemy(Rectangle bounds, int visionSize)
		{
			var vision = Rectangle.FromLTRB(
				bounds.Left - visionSize,
				bounds.Top - visionSize,
				bounds.Right + visionSize,
				bounds.Bottom + visionSize
			);

			PlayMaster.Execute($@"
				playground.CreateEnemy(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					playground.CreateRectangle({vision.X}, {vision.Y}, {vision.Width}, {vision.Height})
				);
			");
		}

		public void Update()
		{
			if (Keyboard.GetPlayerCourse() is { } playerCourse)
			{
				PlayerCharacterSetCourse(playerCourse);
			}

			if (Keyboard.GetPlayerShot() is { } playerShot)
			{
				CreateProjectile(playerShot);
			}

			const int TICKS_COUNT = 10;
			if (Keyboard.IsUndo())
			{
				for (var i = 0; i < TICKS_COUNT; i++)
				{
					PlayMaster.TimeAxis.Revert();
				}
			}
			else
			{
				for (var i = 0; i < TICKS_COUNT; i++)
				{
					PlayMaster.TakeStep();
				}
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
				var textureName = item.DamageSettings.Kind switch
				{
					DamageKind.Fire => Texture.ItemFire,
					DamageKind.Poison => Texture.ItemPoison,
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
				Drawer.Draw(texture, current.Border);
				if (strength is not null)
				{
					Drawer.DrawString(
						strength.Value.ToString(),
						current.Border,
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
				Drawer.Polygon(
					new PointF[]
					{
						new(current.Border.Left, current.Border.Top),
						new(current.Border.Right, current.Border.Top),
						new(current.Border.Right, current.Border.Bottom),
						new(current.Border.Left, current.Border.Bottom),
					},
					color
				);
			}
		}
	}
}