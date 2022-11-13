using Microsoft.Xna.Framework;
using StepFlow.ViewModel;

namespace StepFlow.View.Controlers
{
	public class MovementPieceView : WrappaerView<MovementPieceVm>
	{
		public MovementPieceView(Game1 game, MovementPieceVm source)
			: base(game, source, true)
		{
		}

		public override void Draw(GameTime gameTime)
		{

			base.Draw(gameTime);
		}
	}
}
