﻿using System;
using Microsoft.Xna.Framework.Input;
using StepFlow.Common;
using StepFlow.Markup.Services;

namespace StepFlow.View.Services
{
	public sealed class ControlService : IControl
	{
		[Flags]
		private enum PlayerCourse
		{
			None = 0,
			Left = 0x1,
			Top = 0x2,
			Right = 0x4,
			Bottom = 0x8,
		}

		private KeyboardState PrevKeyboardState { get; set; }

		private MouseState PrevMouseState { get; set; }

		public float? GetPlayerCourse()
		{
			var result = PlayerCourse.None;
			result |= IsKeyDown(Keys.A) ? PlayerCourse.Left : PlayerCourse.None;
			result |= IsKeyDown(Keys.W) ? PlayerCourse.Top : PlayerCourse.None;
			result |= IsKeyDown(Keys.D) ? PlayerCourse.Right : PlayerCourse.None;
			result |= IsKeyDown(Keys.S) ? PlayerCourse.Bottom : PlayerCourse.None;

			return result switch
			{
				PlayerCourse.Left => MathF.PI,
				PlayerCourse.Left | PlayerCourse.Top => -MathF.PI * 3f / 4f,
				PlayerCourse.Top => -MathF.PI / 2,
				PlayerCourse.Right | PlayerCourse.Top => -MathF.PI / 4f,
				PlayerCourse.Right => 0,
				PlayerCourse.Right | PlayerCourse.Bottom => MathF.PI / 4f,
				PlayerCourse.Bottom => MathF.PI / 2,
				PlayerCourse.Left | PlayerCourse.Bottom => MathF.PI * 3f / 4f,
				_ => null,
			};
		}

		public HorizontalAlign GetPlayerCourseHorizontal()
		{
			if (IsKeyDown(Keys.A))
			{
				return HorizontalAlign.Left;
			}
			else if (IsKeyDown(Keys.D))
			{
				return HorizontalAlign.Right;
			}
			else
			{
				return HorizontalAlign.Center;
			}
		}

		public bool GetJump() => IsKeyOnPress(Keys.W);

		public float GetPlayerRotate(System.Numerics.Vector2 center)
		{
			var position = Mouse.GetState().Position;

			var result = MathF.Atan2(
				position.Y - center.Y,
				position.X - center.X
			);

			return result;
		}

		public bool GetPlayerAction() => LeftButtonOnPress;

		public bool IsUndo() => IsKeyDown(Keys.LeftShift);

		public bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

		public bool IsKeyOnPress(Keys key) => Keyboard.GetState().IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key);

		private bool LeftButtonOnPress => Mouse.GetState().LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton != ButtonState.Pressed;

		private bool RightButtonOnPress => Mouse.GetState().RightButton == ButtonState.Pressed && PrevMouseState.RightButton != ButtonState.Pressed;

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

		public bool OnTactic() => IsKeyDown(Keys.Space);

		public System.Drawing.Point FreeSelect() => Mouse.GetState().Position.ToDrawing();

		public System.Drawing.Point FreeOffset() => (Mouse.GetState().Position - PrevMouseState.Position).ToDrawing();

		public SelectCourse CourseSelect()
		{
			if (IsKeyOnPress(Keys.Left))
			{
				return SelectCourse.Left;
			}
			else if (IsKeyOnPress(Keys.Up))
			{
				return SelectCourse.Up;
			}
			else if (IsKeyOnPress(Keys.Right))
			{
				return SelectCourse.Right;
			}
			else if (IsKeyOnPress(Keys.Down))
			{
				return SelectCourse.Down;
			}
			else
			{
				return SelectCourse.None;
			}
		}

		public bool IsSelect() => LeftButtonOnPress;
	}
}
