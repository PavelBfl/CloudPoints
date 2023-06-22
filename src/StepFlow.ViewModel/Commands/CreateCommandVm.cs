using StepFlow.Core;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Commands
{
	public sealed class CreateCommandVm : CommandVm<CreateCommand, Playground, PlaygroundVm>
	{
		public CreateCommandVm(LockProvider wrapperProvider, CreateCommand source)
			: base(wrapperProvider, source)
		{
		}

		public override bool IsMark { get; set; }
	}
}
