using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class AxisVm : Axis<ICommandVm>
	{
		protected override void RegistryHandle(ICommandVm command)
		{
			base.RegistryHandle(command);

			command.Current?.CommandQueue.Add(command);
		}
	}
}
