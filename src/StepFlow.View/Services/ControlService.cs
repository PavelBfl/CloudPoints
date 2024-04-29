using System;
using Microsoft.Xna.Framework.Input;
using StepFlow.Core;
using StepFlow.Markup.Services;

namespace StepFlow.View.Services
{
	public sealed class ControlService : IControl
	{
		private KeyboardState PrevKeyboardState { get; set; }

		private MouseState PrevMouseState { get; set; }

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

		public float GetPlayerRotate(System.Numerics.Vector2 center)
		{
			var position = Mouse.GetState().Position;

			var result = MathF.Atan2(
				position.Y - center.Y,
				position.X - center.X
			);

			return result;
		}

		public PlayerAction GetPlayerAction()
		{
			if (LeftButtonOnPress)
			{
				return PlayerAction.Default;
			}
			else
			{
				return PlayerAction.None;
			}
		}

		public bool IsUndo() => IsKeyDown(Keys.LeftShift);

		public bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

		public bool IsKeyOnPress(Keys key) => Keyboard.GetState().IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key);

		private bool LeftButtonOnPress => Mouse.GetState().LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton != ButtonState.Pressed;

		public void Update()
		{
			PrevKeyboardState = Keyboard.GetState();
			PrevMouseState = Mouse.GetState();
		}

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
