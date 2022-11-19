using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controlers
{
	public abstract class PolygonBase : ViewBase
	{
		protected PolygonBase(Game1 game) : base(game)
		{
		}

		public abstract IReadOnlyList<Vector2> GetVertices();

		public Microsoft.Xna.Framework.Color Color { get; set; } = Microsoft.Xna.Framework.Color.Black;

		public override void Draw(GameTime gameTime)
		{
			Game.SpriteBatch.DrawPolygon(GetVertices(), Color);

			base.Draw(gameTime);
		}
	}
}
