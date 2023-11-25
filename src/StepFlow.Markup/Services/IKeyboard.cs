using StepFlow.Core;

namespace StepFlow.Markup.Services
{
	public interface IKeyboard
	{
		Course? GetPlayerCourse();

		Course? GetPlayerShot();

		bool IsUndo();
	}
}
