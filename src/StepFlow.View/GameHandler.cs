using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
		}

		private PlayMasterVm PlayMaster { get; }

		private Primitive Base { get; }

		private SpriteFont Font { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

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

			Update(Base, gameTime);

			KeyboardService.Update();
			MouseService.Update();
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
					var polygon = new Polygon(Base.ServiceProvider)
					{
						Color = subject.IsSelect ? Color.Green : Color.Red,
						Vertices = new BoundsVertices()
						{
							Bounds = collided.Current.Border,
						},
					};
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