using Microsoft.Xna.Framework;

namespace StepFlow.View.Services
{
	public interface IMouseService
	{
		Point Position { get; }
		bool LeftButtonOnPress { get; }
	}
}
