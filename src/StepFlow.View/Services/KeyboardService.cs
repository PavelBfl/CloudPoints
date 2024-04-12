using Microsoft.Xna.Framework.Input;
using StepFlow.Core;
using StepFlow.Markup.Services;

namespace StepFlow.View.Services
{
	public class KeyboardService : IKeyboard
	{
		private KeyboardState PrevKeyboardState { get; set; }

		public Course? GetPlayerCourse()
		{
			if (IsKeyDown(Keys.Left))
			{
				return Course.Left;
			}
			else if (IsKeyDown(Keys.Up))
			{
				return Course.Top;
			}
			else if (IsKeyDown(Keys.Right))
			{
				return Course.Right;
			}
			else if (IsKeyDown(Keys.Down))
			{
				return Course.Bottom;
			}
			else
			{
				return null;
			}
		}

		public Course? GetPlayerShot()
		{
			if (IsKeyDown(Keys.A))
			{
				return Course.Left;
			}
			else if (IsKeyDown(Keys.W))
			{
				return Course.Top;
			}
			else if (IsKeyDown(Keys.D))
			{
				return Course.Right;
			}
			else if (IsKeyDown(Keys.S))
			{
				return Course.Bottom;
			}
			else
			{
				return null;
			}
		}

		public bool IsUndo() => IsKeyDown(Keys.LeftShift);

		public bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

		public bool IsKeyOnPress(Keys key) => Keyboard.GetState().IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key);

		public void Update() => PrevKeyboardState = Keyboard.GetState();

		public TimeOffset GetTimeOffset()
		{
			if (IsKeyOnPress(Keys.OemMinus))
			{
				return TimeOffset.Down;
			}
			else if (IsKeyOnPress(Keys.OemPlus))
			{
				return TimeOffset.Up;
			}
			else
			{
				return TimeOffset.None;
			}
		}

		public bool OnSwitchDebug() => IsKeyOnPress(Keys.Tab);
	}
}
