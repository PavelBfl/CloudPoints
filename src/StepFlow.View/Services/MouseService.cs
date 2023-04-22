using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StepFlow.View.Services
{
	public class MouseService : IMouseService
	{
		private MouseState PrevMouseState { get; set; }

		public Point Position => Mouse.GetState().Position;

		public bool LeftButtonOnPress => Mouse.GetState().LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton != ButtonState.Pressed;

		public void Update() => PrevMouseState = Mouse.GetState();
	}
}
