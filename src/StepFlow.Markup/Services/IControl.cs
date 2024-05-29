using System.Numerics;
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

		float GetPlayerRotate(Vector2 center);

		PlayerAction? GetPlayerAction();

		bool IsUndo();

		TimeOffset GetTimeOffset();

		bool OnSwitchDebug();

		bool OnTactic();

		Vector2? FreeSelect();
	}
}
