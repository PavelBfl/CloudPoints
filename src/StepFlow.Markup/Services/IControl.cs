using StepFlow.Core;

namespace StepFlow.Markup.Services
{
	public enum TimeOffset
	{
		None,
		Up,
		Down,
	}

	public enum PlayerAction
	{
		None,
		Default,
	}

	public interface IControl
	{
		Course? GetPlayerCourse();

		float GetPlayerRotate(System.Numerics.Vector2 center);

		PlayerAction GetPlayerAction();

		bool IsUndo();

		TimeOffset GetTimeOffset();

		bool OnSwitchDebug();
	}
}
