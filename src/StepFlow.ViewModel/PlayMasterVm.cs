using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Master;
using StepFlow.ViewModel.Collections;
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

		public void Execute(string script) => Source.Execute(script);

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(playground);
	}

	public sealed class PlaygroundVm : WrapperVm<Playground>
	{
		public PlaygroundVm(LockProvider lockProvider, Playground source) : base(lockProvider, source)
		{
			Subjects = new WrapperCollection<SubjectVm, IReadOnlyCollection<Subject>, Subject>(
				LockProvider,
				(IReadOnlyCollection<Subject>)Source.Subjects
			);
		}

		public WrapperCollection<SubjectVm, IReadOnlyCollection<Subject>, Subject> Subjects { get; }

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(Subjects);
	}

	public sealed class SubjectVm : WrapperVm<Subject>
	{
		public SubjectVm(LockProvider lockProvider, Subject source) : base(lockProvider, source)
		{
		}

		public bool IsSelect { get; set; }
	}
}
