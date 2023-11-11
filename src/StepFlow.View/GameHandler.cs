using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.Core.Components;
using StepFlow.Master;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Elements;
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
			Init();
		}

		private Meter Meter { get; } = new Meter("Game.Gameplay");

		private PlayMaster PlayMaster { get; } = new PlayMaster();

		private Primitive Base { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

		public void Init()
		{
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

		public void Update(GameTime gameTime)
		{
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

			var playground = PlayMaster.GetPlaygroundProxy();

			if (CreateTexture(playground.PlayerCharacter, "Character") is { } playerCharacter)
			{
				Base.Childs.Add(playerCharacter);
			}

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			Draw(Base, gameTime);

			SpriteBatch.End();
		}

		private Primitive? CreateTexture(IProxyBase<ICollided>? collided, string textureName)
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