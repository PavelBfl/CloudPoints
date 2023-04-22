using Microsoft.Xna.Framework.Input;

namespace StepFlow.View.Services
{
	public class KeyboardService : IKeyboardService
	{
		private KeyboardState PrevKeyboardState { get; set; }

		public bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

		public bool IsKeyOnPress(Keys key) => Keyboard.GetState().IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key);

		public void Update() => PrevKeyboardState = Keyboard.GetState();
	}
}
