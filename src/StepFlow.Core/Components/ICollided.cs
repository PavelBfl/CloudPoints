using StepFlow.Core.Border;

namespace StepFlow.Core.Components
{
	public interface ICollided
	{
		Bordered? Current { get; set; }

		Bordered? Next { get; set; }

		bool IsMove { get; set; }
	}
}
