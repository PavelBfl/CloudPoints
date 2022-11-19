using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StepFlow.ViewModel;

namespace StepFlow.View.Controlers
{
	public class AxisView : WrappaerView<AxisVm>
	{
		public AxisView(Game1 game, AxisVm source)
			: base(game, source, true)
		{
		}

		public override void Update(GameTime gameTime)
		{
			if (Game.IsKeyOnPress(Keys.Space))
			{
				Source.MoveNext();
			}

			base.Update(gameTime);
		}
	}
}
