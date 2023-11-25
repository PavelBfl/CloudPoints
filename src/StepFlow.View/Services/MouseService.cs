using Microsoft.Xna.Framework.Input;
using StepFlow.Markup.Services;

namespace StepFlow.View.Services
{
	public class MouseService : IMouse
	{
		private MouseState PrevMouseState { get; set; }

		public System.Drawing.Point Position
		{
			get
			{
				var monoPosition = Mouse.GetState().Position;
				return new(monoPosition.X, monoPosition.Y);
			}
		}

		public bool LeftButtonOnPress => Mouse.GetState().LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton != ButtonState.Pressed;

		public void Update() => PrevMouseState = Mouse.GetState();
	}
}
