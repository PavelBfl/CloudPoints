using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface ICommandVm : ICommand, IMarkered
	{
		PieceVm? Current { get; }
	}
}
