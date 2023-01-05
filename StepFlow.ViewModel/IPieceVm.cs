using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface IPieceVm : IMarkered
	{
		WorldVm Owner { get; }
		CommandsQueueVm CommandQueue { get; }
		HexNodeVm? Current { get; set; }

		void Add(ICommandVm command);

		void MoveTo(HexNodeVm node);
	}
}
