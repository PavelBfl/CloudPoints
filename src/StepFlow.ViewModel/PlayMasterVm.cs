using System.Collections.Generic;
using StepFlow.Master;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public sealed class PlayMasterVm : WrapperVm<PlayMaster>
	{
		public PlayMasterVm(LockProvider lockProvider, PlayMaster source) : base(lockProvider, source)
		{
		}

		private PlaygroundVm? playground;

		public PlaygroundVm Playground => playground ??= LockProvider.GetOrCreate<PlaygroundVm>(Source.Playground);

		public void TakeStep() => Source.TakeStep();

		public void Execute(string script) => Source.Execute(script);

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(playground);
	}
}
