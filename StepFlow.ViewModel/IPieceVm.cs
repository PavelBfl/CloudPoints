using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface IPieceVm
	{
		CommandsQueueVm CommandQueue { get; }

		void Add(ICommand command);

		void MoveTo(HexNodeVm node);
	}
}
