using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master;
using StepFlow.Master.Proxies.Components;
using StepFlow.View.Controls;
using StepFlow.View.Services;
using StepFlow.View.Sketch;

namespace StepFlow.View
{
	public class GameHandler
	{
		public GameHandler(Game1 game, System.Drawing.RectangleF bounds)
		{
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);

			Drawer = new Drawer(SpriteBatch, game.GraphicsDevice, game.Content);

			game.Services.AddService<IMouseService>(MouseService);
			game.Services.AddService<IKeyboardService>(KeyboardService);
			game.Services.AddService<IDrawer>(Drawer);

			Place = bounds;

			Base = new Primitive(game.Services);

			Meter.CreateObservableGauge("Time", () => PlayMaster.Time);
			Meter.CreateObservableGauge("FPS", () => Fps);
			Meter.CreateObservableGauge("Slow", () => Slow);
			Meter.CreateObservableGauge("Mpf", () => Mpf);
			Init();
		}

		private Meter Meter { get; } = new Meter("Game.Gameplay");

		private double Fps { get; set; }

		private int Slow { get; set; }

		private double Mpf { get; set; }

		private PlayMaster PlayMaster { get; } = new PlayMaster();

		private Primitive Base { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

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

		private void CreatePlayerCharacter(System.Drawing.Rectangle bounds, float strength)
		{
			PlayMaster.Execute(@$"
				playground.CreatePlayerCharacter(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					{strength}
				);
			");
		}

		private void CreateRoom(System.Drawing.Point location, System.Drawing.Size size, int width)
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

			void CreateBounds(System.Drawing.Point index)
			{
				var tileLocation = new System.Drawing.Point(index.X * width + location.X, index.Y * width + location.Y);
				var bounds = new System.Drawing.Rectangle(tileLocation.X, tileLocation.Y, width, width);
				CreateObstruction(bounds, null);
			}
		}

		private void CreateObstruction(System.Drawing.Rectangle bounds, float? strength)
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

		private void CreateItem(System.Drawing.Rectangle bounds, int value, DamageKind kind)
		{
			PlayMaster.Execute($@"
				playground.CreateItem(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					{value},
					{(int)kind}
				);
			");
		}

		private void CreateEnemy(System.Drawing.Rectangle bounds, int visionSize)
		{
			var vision = System.Drawing.Rectangle.FromLTRB(
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

		public void Update(GameTime gameTime)
		{
			var sw = Stopwatch.StartNew();

			if (KeyboardService.IsKeyDown(Keys.Left))
			{
				PlayerCharacterSetCourse(Course.Left);
			}
			else if (KeyboardService.IsKeyDown(Keys.Up))
			{
				PlayerCharacterSetCourse(Course.Top);
			}
			else if (KeyboardService.IsKeyDown(Keys.Right))
			{
				PlayerCharacterSetCourse(Course.Right);
			}
			else if (KeyboardService.IsKeyDown(Keys.Down))
			{
				PlayerCharacterSetCourse(Course.Bottom);
			}

			if (KeyboardService.IsKeyDown(Keys.A))
			{
				CreateProjectile(Course.Left);
			}
			else if (KeyboardService.IsKeyDown(Keys.W))
			{
				CreateProjectile(Course.Top);
			}
			else if (KeyboardService.IsKeyDown(Keys.D))
			{
				CreateProjectile(Course.Right);
			}
			else if (KeyboardService.IsKeyDown(Keys.S))
			{
				CreateProjectile(Course.Bottom);
			}

			Update(Base, gameTime);

			const int TICKS_COUNT = 10;
			if (KeyboardService.IsKeyDown(Keys.LeftShift))
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

			KeyboardService.Update();
			MouseService.Update();

			Fps = TimeSpan.FromSeconds(1) / gameTime.ElapsedGameTime;
			Slow = ((IConvertible)gameTime.IsRunningSlowly).ToInt32(null);
			Mpf = sw.Elapsed.TotalMilliseconds;
		}

		private static void Update(Primitive primitive, GameTime gameTime)
		{
			if (primitive.Enable)
			{
				primitive.Update(gameTime);

				foreach (var child in primitive.Childs)
				{
					Update(child, gameTime);
				}
			}
		}

		public void Draw(GameTime gameTime)
		{
			Base.Childs.Clear();

			var floorTileSize = new Vector2(40);
			for (var iX = 0f; iX < Place.Width; iX += floorTileSize.X)
			{
				for (var iY = 0f; iY < Place.Height; iY += floorTileSize.Y)
				{
					var tile = new TextureLayout(Base.ServiceProvider)
					{
						Name = "Floor",
						Layout = new Layout()
						{
							Owner = Place,
							Margin = new Thickness()
							{
								Left = iX,
								Top = iY,
								Right = UnitKind.None,
								Bottom = UnitKind.None,
							},
							Size = new(floorTileSize.X, floorTileSize.Y),
						},
					};

					Base.Childs.Add(tile);
				}
			}

			var playground = PlayMaster.GetPlaygroundProxy();

			if (CreateTexture(playground.PlayerCharacter?.Body, "Character", null) is { } playerCharacter)
			{
				Base.Childs.Add(playerCharacter);
			}

			foreach (var barrier in playground.Obstructions)
			{
				if (CreateTexture(barrier.Body, "Wall", barrier.Strength) is { } wall)
				{
					Base.Childs.Add(wall);
				}
			}

			foreach (var projectile in playground.Projectiles)
			{
				var textureName = projectile.Damage?.Kind switch
				{
					DamageKind.None => "Projectile",
					DamageKind.Fire => "ProjectileFire",
					DamageKind.Poison => "ProjectilePoison",
					DamageKind.Fire | DamageKind.Poison => "ProjectileAll",
					_ => "Projectile",
				};

				if (CreateTexture(projectile.Body, textureName, null) is { } instance)
				{
					Base.Childs.Add(instance);
				}
			}

			foreach (var item in playground.Items)
			{
				var textureName = item.DamageSettings.Kind switch
				{
					DamageKind.Fire => "ItemFire",
					DamageKind.Poison => "ItemPoison",
					_ => "ItemUnknown",
				};

				if (CreateTexture(item.Body, textureName, null) is { } instance)
				{
					Base.Childs.Add(instance);
				}
			}

			foreach (var enemy in playground.Enemies)
			{
				if (CreateTexture(enemy.Body, "Enemy", null) is { } instance)
				{
					var border = enemy.Vision.Current.Border;
					instance.Childs.Add(new LayoutControl(Base.ServiceProvider)
					{
						BoundsColor = Color.Yellow,
						Layout = new Layout()
						{
							Owner = Place,
							Margin = new()
							{
								Left = border.Left,
								Top = border.Top,
								Right = UnitKind.None,
								Bottom = UnitKind.None,
							},
							Size = new(border.Width, border.Height),
						},
					});

					Base.Childs.Add(instance);
				}
			}

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			Draw(Base, gameTime);

			SpriteBatch.End();
		}

		private Primitive? CreateTexture(ICollidedProxy? collided, string textureName, IScaleProxy? strength)
		{
			if (collided?.Target is { Current: { } current })
			{
				var root = new Primitive(Base.ServiceProvider);
				var layout = new Layout()
				{
					Owner = Place,
					Margin = new()
					{
						Left = current.Border.Left,
						Top = current.Border.Top,
						Right = UnitKind.None,
						Bottom = UnitKind.None,
					},
					Size = new(current.Border.Width, current.Border.Height),
				};

				root.Childs.Add(new LayoutControl(Base.ServiceProvider)
				{
					Layout = new Layout()
					{
						Owner = layout.Place,
					},
				});

				root.Childs.Add(new TextureLayout(Base.ServiceProvider)
				{
					Name = textureName,
					Layout = new Layout()
					{
						Owner = layout.Place,
					},
				});

				if (strength is not null)
				{
					root.Childs.Add(new Text(Base.ServiceProvider)
					{
						Layout = new Layout()
						{
							Owner = layout.Place
						},
						HorizontalAlign = HorizontalAlign.Center,
						VerticalAlign = VerticalAlign.Center,
						Content = strength.Value.ToString(),
						Color = Color.Red,
					}); 
				}

				return root;
			}
			else
			{
				return null;
			}
		}

		private static void Draw(Primitive primitive, GameTime gameTime)
		{
			if (primitive.Visible)
			{
				primitive.Draw(gameTime);

				foreach (var child in primitive.Childs)
				{
					Draw(child, gameTime);
				}
			}
		}
	}
}