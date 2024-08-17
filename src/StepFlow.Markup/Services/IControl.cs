﻿using System.Numerics;
using StepFlow.Common;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Markup.Services
{
	public enum TimeOffset
	{
		None,
		Up,
		Down,
	}

	public interface IControl
	{
		float? GetPlayerCourse();

		HorizontalAlign GetPlayerCourseHorizontal();

		bool GetJump();

		float GetPlayerRotate(Vector2 center);

		PlayerAction? GetPlayerAction();

		bool IsUndo();

		TimeOffset GetTimeOffset();

		bool OnSwitchDebug();

		bool OnTactic();

		Vector2? FreeSelect();

		bool SelectMain();

		bool SelectAuxiliary();
	}
}
