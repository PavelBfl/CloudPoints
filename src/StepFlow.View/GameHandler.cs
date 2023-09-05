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
	public class GameHandler : ILayoutCanvas
	{
		public GameHandler(Game1 game, System.Drawing.RectangleF bounds)
		{
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);

			Font = game.Content.Load<SpriteFont>("DefaultFont");

			Drawer = new Drawer(SpriteBatch, game.GraphicsDevice);

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

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

		public void Init()
		{
			CreateRoom(new(0, 0, 200, 100), 10);

			PlayMaster.Execute(@$"
				subject = playground.CreateSubject()
				subject.AddComponent(""Collided"")
				collided = subject.GetComponent(""Collided"")
				bordered = playground.CreateBordered()
				bordered.AddCell(playground.CreateRectangle(40, 40, 20, 20))
				collided.Current = bordered
				subject.AddComponent(""Strength"")
				strength = subject.GetComponent(""Strength"")
				strength.Max = 100
				strength.Value = 100
				subject.AddComponent(""Scheduler"")
				playground.Subjects.Add(subject)
			");
		}


		public void CreateRoom(Rectangle rectangle, int width)
		{
			CreateWall(new(rectangle.X, rectangle.Y, rectangle.Width, width));
			CreateWall(new(rectangle.X, rectangle.Y, width, rectangle.Height));
			CreateWall(new(rectangle.Right - width, rectangle.Y, width, rectangle.Height));
			CreateWall(new(rectangle.X, rectangle.Bottom - width, rectangle.Width, width));
		}
		public void CreateWall(Rectangle rectangle)
		{
			PlayMaster.Execute(@$"
				subject = playground.CreateSubject()
				subject.AddComponent(""Collided"")
				collided = subject.GetComponent(""Collided"")
				bordered = playground.CreateBordered()
				bordered.AddCell(playground.CreateRectangle({rectangle.X}, {rectangle.Y}, {rectangle.Width}, {rectangle.Height}))
				collided.Current = bordered
				playground.Subjects.Add(subject)
			");
		}

		public void Update(GameTime gameTime)
		{
			if (KeyboardService.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Space))
			{
				PlayMaster.Execute(@$"
					subject = playground.CreateSubject()
					subject.AddComponent(""Collided"")
					collided = subject.GetComponent(""Collided"")
					bordered = playground.CreateBordered()
					bordered.AddCell(playground.CreateRectangle({Random.Shared.Next(100)}, {Random.Shared.Next(100)}, {Random.Shared.Next(10, 100)}, {Random.Shared.Next(10, 100)}))
					collided.Current = bordered
					collided.Offset(playground.CreatePoint(5, 5))
					collided.Damage = 5
					subject.AddComponent(""Strength"")
					strength = subject.GetComponent(""Strength"")
					strength.Max = 100
					strength.Value = 100
					playground.Subjects.Add(subject)
				");
			}

			if (KeyboardService.IsKeyOnPress(Keys.Tab))
			{
				var subjects = PlayMaster.Playground.Subjects.ToArray();
				var current = Array.FindIndex(subjects, x => x.Source.Components[Playground.STRENGTH_NAME] is not null);
				if (current >= 0)
				{
					subjects[current].IsSelect = true;
				}
			}

			if (KeyboardService.IsKeyOnPress(Keys.Up) && KeyboardService.IsKeyOnPress(Keys.Right))
			{
				SetCourse(Course.RightTop);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.Up) && KeyboardService.IsKeyOnPress(Keys.Left))
			{
				SetCourse(Course.LeftTop);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.Down) && KeyboardService.IsKeyOnPress(Keys.Right))
			{
				SetCourse(Course.RightBottom);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.Down) && KeyboardService.IsKeyOnPress(Keys.Left))
			{
				SetCourse(Course.LeftBottom);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.Up))
			{
				SetCourse(Course.Top);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.Right))
			{
				SetCourse(Course.Right);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.Down))
			{
				SetCourse(Course.Bottom);
			}
			else if (KeyboardService.IsKeyOnPress(Keys.Left))
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

			if (PlayMaster.Source.Playground.Subjects
				.Select(x => x.Components[Playground.SCHEDULER_NAME])
				.OfType<Scheduled>()
				.Where(x => x.Queue.Any()).Any())
			{
				PlayMaster.TakeStep();
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
				PlayMaster.Execute($@"
						scheduler = playground.Subjects[{current}].GetComponent(""Scheduler"")
						if scheduler != null then
							scheduler.SetCourse({(int)value})
						end
					");
			}
		}

		private void CreateProjectile(Course course)
		{
			var subjects = PlayMaster.Playground.Subjects.ToArray();
			var current = Array.FindIndex(subjects, x => x.IsSelect);
			if (current >= 0)
			{
				PlayMaster.Execute($@"
						scheduler = playground.Subjects[{current}].GetComponent(""Scheduler"")
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
				if (subject.Source.Components[Playground.COLLIDED_NAME] is Collided collided && collided.Current is not null)
				{
					var color = subject.IsSelect ? Color.Green : Color.Red;
					var polygon = new Polygon(Base.ServiceProvider)
					{
						Color = color,
						Vertices = new BoundsVertices()
						{
							Bounds = collided.Current.Border,
						},
					};

					if (subject.Source.Components["Strength"] is Scale strength)
					{
						var border = collided.Current.Border;
						var text = new Text(Base.ServiceProvider)
						{
							Color = color,
							Font = Font,
							Content = strength.Value.ToString(),
							VerticalAlign = VerticalAlign.Center,
							HorizontalAlign = HorizontalAlign.Center,
							Layout = new Layout()
							{
								Canvas = this,
								Margin = new()
								{
									Left = new Unit(border.X),
									Top = new Unit(border.Y),
									Right = new Unit(0, UnitKind.None),
									Bottom = new Unit(0, UnitKind.None),
								},
								Size = new System.Drawing.SizeF(border.Width, border.Height),
							}
						};
						polygon.Childs.Add(text);
					}

					Base.Childs.Add(polygon);
				}
			}

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			Draw(Base, gameTime);

			SpriteBatch.End();
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