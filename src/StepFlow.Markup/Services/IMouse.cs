using System.Drawing;

namespace StepFlow.Markup.Services
{
	public interface IMouse
	{
		Point Position { get; }

		bool LeftButtonOnPress { get; }
	}
}
