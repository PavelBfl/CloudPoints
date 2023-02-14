using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface IPieceVm : IParticleVm, IMarkered
	{
		CommandsQueueVm CommandQueue { get; }
		NodeVm? Current { get; set; }
		NodeVm? Next { get; set; }

		void Add(ICommandVm command);

		void MoveTo(NodeVm node);
		void TakeStep();
	}
}
