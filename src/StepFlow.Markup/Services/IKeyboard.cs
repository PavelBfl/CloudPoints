using StepFlow.Core;

namespace StepFlow.Markup.Services
{
	public enum TimeOffset
	{
		None,
		Up,
		Down,
	}

	public interface IKeyboard
	{
		Course? GetPlayerCourse();

		Course? GetPlayerShot();

		bool IsUndo();

		TimeOffset GetTimeOffset();

		bool OnSwitchDebug();
	}
}
