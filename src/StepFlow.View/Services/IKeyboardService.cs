using Microsoft.Xna.Framework.Input;

namespace StepFlow.View.Services
{
	public interface IKeyboardService
	{
		bool IsKeyDown(Keys key);
		bool IsKeyOnPress(Keys key);
	}
}
