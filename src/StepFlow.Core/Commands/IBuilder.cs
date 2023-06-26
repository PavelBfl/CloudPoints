using StepFlow.TimeLine;

namespace StepFlow.Core.Commands
{
	public interface IBuilder
	{
		ICommand Build(object target);

		bool CanBuild(object target);
	}
}
