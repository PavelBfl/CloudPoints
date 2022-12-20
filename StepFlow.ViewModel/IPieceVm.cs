using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface IPieceVm : ISelectable
	{
		CommandsQueueVm CommandQueue { get; }

		void Add(ICommand command);

		void MoveTo(HexNodeVm node);
	}
}
