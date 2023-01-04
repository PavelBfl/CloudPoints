using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface ICommandVm : ICommand, ISelectable
	{
		IPieceVm? Current { get; }
	}
}
