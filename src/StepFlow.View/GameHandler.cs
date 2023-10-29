using System;
using System.Diagnostics.Metrics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master;
using StepFlow.View.Controls;
using StepFlow.View.Services;
using StepFlow.View.Sketch;
using StepFlow.ViewModel;
using StepFlow.ViewModel.Collector;

namespace StepFlow.View
{
	public class GameHandler
	{
		public GameHandler(Game1 game, System.Drawing.RectangleF bounds)
		{
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);

			Font = game.Content.Load<SpriteFont>("DefaultFont");
			Character = game.Content.Load<Texture2D>("Character");

			Drawer = new Drawer(SpriteBatch, game.GraphicsDevice, game.Content);

			game.Services.AddService<IMouseService>(MouseService);
			game.Services.AddService<IKeyboardService>(KeyboardService);
			game.Services.AddService<IDrawer>(Drawer);

			Place = bounds;

			Base = new Primitive(game.Services);

			PlayMaster = new PlayMasterVm(new LockProvider(), new PlayMaster());
			Meter.CreateObservableGauge("Time", () => PlayMaster.Source.Time);
			Init();
		}

		private Meter Meter { get; } = new Meter("Game.Gameplay");

		private PlayMasterVm PlayMaster { get; }

		private Primitive Base { get; }

		private SpriteFont Font { get; }

		private Texture2D Character { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

		public void Init()
		{
			CreateRoom(new(0, 0, 400, 200), 10);

			CreateCharacter(new(40, 40, 20, 20));
			CreateCharacter(new(100, 40, 20, 20));
			CreateCharacter(new(50, 100, 35, 35));

			CreateItem(new(30, 30, 5, 5), Master.PlayMaster.FIRE_DAMAGE);
			CreateItem(new(60, 30, 5, 5), Master.PlayMaster.POISON_DAMAGET);

			CreateSentryGun(new(170, 80, 10, 10), 50);
		}

		private void CreateCharacter(Rectangle rectangle, int strength = 100)
		{
			PlayMaster.Execute(@$"
				rectangle = playground.CreateRectangle({rectangle.X}, {rectangle.Y}, {rectangle.Width}, {rectangle.Height})
				playground.CreateCharacter(rectangle, {strength})
			");
		}

		private void CreateRoom(Rectangle rectangle, int width)
		{
			PlayMaster.Execute(@$"
				rectangle = playground.CreateRectangle({rectangle.X}, {rectangle.Y}, {rectangle.Width}, {rectangle.Height})
				playground.CreateRoom(rectangle, {width})
			");
		}

		private void CreateItem(Rectangle rectangle, string kind)
		{
			PlayMaster.Execute(@$"
				rectangle = playground.CreateRectangle({rectangle.X}, {rectangle.Y}, {rectangle.Width}, {rectangle.Height})
				playground.CreateItem(rectangle, ""{kind}"")
			");
		}

		private void CreateSentryGun(Rectangle size, int visionRadius, int strength = 50)
		{
			var center = size.Center;
			var vision = new Rectangle(
				center.X - visionRadius,
				center.Y - visionRadius,
				visionRadius * 2,
				visionRadius * 2
			);

			PlayMaster.Execute($@"
				size = playground.CreateRectangle({size.X}, {size.Y}, {size.Width}, {size.Height})
				vision = playground.CreateRectangle({vision.X}, {vision.Y}, {vision.Width}, {vision.Height})
				playground.CreateSentryGun(size, vision, {strength})
			");
		}

		public void Update(GameTime gameTime)
		{
			if (KeyboardService.IsKeyOnPress(Keys.Tab))
			{
				var subjects = PlayMaster.Playground.Subjects.ToArray();
				var current = Array.FindIndex(subjects, x => x.Source.Components[Components.Names.STRENGTH] is not null);
				if (current >= 0)
				{
					subjects[current].IsSelect = true;
				}
			}

			if (KeyboardService.IsKeyDown(Keys.Up) && KeyboardService.IsKeyDown(Keys.Right))
			{
				SetCourse(Course.RightTop);
			}
			else if (KeyboardService.IsKeyDown(Keys.Up) && KeyboardService.IsKeyDown(Keys.Left))
			{
				SetCourse(Course.LeftTop);
			}
			else if (KeyboardService.IsKeyDown(Keys.Down) && KeyboardService.IsKeyDown(Keys.Right))
			{
				SetCourse(Course.RightBottom);
			}
			else if (KeyboardService.IsKeyDown(Keys.Down) && KeyboardService.IsKeyDown(Keys.Left))
			{
				SetCourse(Course.LeftBottom);
			}
			else if (KeyboardService.IsKeyDown(Keys.Up))
			{
				SetCourse(Course.Top);
			}
			else if (KeyboardService.IsKeyDown(Keys.Right))
			{
				SetCourse(Course.Right);
			}
			else if (KeyboardService.IsKeyDown(Keys.Down))
			{
				SetCourse(Course.Bottom);
			}
			else if (KeyboardService.IsKeyDown(Keys.Left))
			{
				SetCourse(Course.Left);
			}

			if (KeyboardService.IsKeyOnPress(Keys.A))
			{
				CreateProjectile(Course.Left);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.W))
			{
				CreateProjectile(Course.Top);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.D))
			{
				CreateProjectile(Course.Right);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.S))
			{
				CreateProjectile(Course.Bottom);
			}

			for (int i = 0; i < 10; i++)
			{
				if (PlayMaster.Source.Playground.Subjects
					.SelectMany(x => x.Components.OfType<Scheduled>())
					.Where(x => x.Queue.Any()).Any())
				{
					PlayMaster.TakeStep();
				}
			}

			Update(Base, gameTime);

			KeyboardService.Update();
			MouseService.Update();
		}

		private void SetCourse(Course value)
		{
			var subjects = PlayMaster.Playground.Subjects.ToArray();
			var current = Array.FindIndex(subjects, x => x.IsSelect);
			if (current >= 0)
			{
				if (subjects[current].Source.Components["MainScheduler"] is Scheduled scheduled)
				{
					if (scheduled.Queue.LastOrDefault().Executor?.Reference != nameof(Handlers.SetCourseHandler))
					{
						PlayMaster.Execute($@"
								scheduler = playground.Subjects[{current}].GetComponent(""MainScheduler"")
								if scheduler != null then
									scheduler.SetCourse({(int)value}, 10)
								end
							");
					}
				}

			}
		}

		private void CreateProjectile(Course course)
		{
			var subjects = PlayMaster.Playground.Subjects.ToArray();
			var current = Array.FindIndex(subjects, x => x.IsSelect);
			if (current >= 0)
			{
				PlayMaster.Execute($@"
						scheduler = playground.Subjects[{current}].GetComponent(""MainScheduler"")
						if scheduler != null then
							scheduler.CreateProjectile({(int)course})
						end
					");
			}
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
			foreach (var subject in PlayMaster.Playground.Subjects)
			{
				DrawBorder(subject, Components.Names.COLLIDED, subject.IsSelect ? Color.Green : Color.Red, subject.IsSelect);
				DrawBorder(subject, Components.Names.VISION, Color.Yellow, false);
			}

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			Draw(Base, gameTime);

			SpriteBatch.End();
		}

		private void DrawBorder(SubjectVm subject, string name, Color color, bool isCharacter)
		{
			if (subject.Source.Components[name] is Collided collided && collided.Current is not null)
			{
				var border = collided.Current.Border;
				var polygon = new Polygon(Base.ServiceProvider)
				{
					Color = color,
					Vertices = new BoundsVertices()
					{
						Bounds = collided.Current.Border,
					},
				};

				if (isCharacter)
				{
					polygon.Childs.Add(new Controls.Texture(Base.ServiceProvider)
					{
						Texture2D = Character,
						Rectangle = new(border.X, border.Y, border.Width, border.Height),
					}); 
				}

				if (subject.Source.Components["Strength"] is Scale strength)
				{
					var text = new Text(Base.ServiceProvider)
					{
						Color = color,
						Font = Font,
						Content = strength.Value.ToString(),
						VerticalAlign = VerticalAlign.Center,
						HorizontalAlign = HorizontalAlign.Center,
						Layout = new Layout()
						{
							Owner = border,
						}
					};
					polygon.Childs.Add(text);
				}

				Base.Childs.Add(polygon);
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