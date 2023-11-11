using StepFlow.Core.Border;

namespace StepFlow.Core.Components
{
	public interface ICollided
	{
		IBordered? Current { get; set; }

		IBordered? Next { get; set; }

		bool IsMove { get; set; }
	}
}
