using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public class Control : DrawableGameComponent
	{
		public Control(Game game)
			: base(game)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Game.Components.Remove(this);
			}

			base.Dispose(disposing);
		}
	}
}
