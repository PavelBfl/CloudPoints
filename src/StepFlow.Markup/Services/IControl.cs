using System.Numerics;
using StepFlow.Common;

namespace StepFlow.Markup.Services
{
	public enum TimeOffset
	{
		None,
		Up,
		Down,
	}

	public enum SelectCourse
	{
		None,
		Left,
		Up,
		Right,
		Down,
	}

	public interface IControl
	{
		HorizontalAlign GetPlayerCourseHorizontal();

		bool GetJump();

		float GetPlayerRotate(Vector2 center);

		bool IsUndo();

		TimeOffset GetTimeOffset();

		bool OnSwitchDebug();

		bool OnTactic();

		#region Panel
		System.Drawing.Point FreeSelect();

		System.Drawing.Point FreeOffset();

		SelectCourse CourseSelect();

		bool IsSelect();
		bool GetPlayerAction();
		#endregion
	}
}
