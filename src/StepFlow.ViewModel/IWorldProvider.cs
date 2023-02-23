using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public interface IWorldProvider
	{
		WorldVm GetWorld(World world);
	}
}
