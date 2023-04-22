using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.View.Services;

namespace StepFlow.View.Controls
{
	public class MouseHandler : GameComponent
	{
		public MouseHandler(Game game) : base(game)
		{
			MouseService = Game.Services.GetService<IMouseService>();
		}

		public IMouseService MouseService { get; }

		public IReadOnlyVertices? Vertices { get; set; }

		public bool MouseContains => (Vertices?.Any() ?? false) && Vertices.FillContains(MouseService.Position);

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (OnMouseClick is { } && (Vertices?.Any() ?? false) && Vertices.FillContains(MouseService.Position))
			{
				OnMouseClick(this, EventArgs.Empty);
			}
		}

		public event EventHandler? OnMouseClick;
	}
}
