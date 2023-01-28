using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface ICommandVm : ICommand, IMarkered
	{
		IPieceVm? Current { get; }
	}
}
