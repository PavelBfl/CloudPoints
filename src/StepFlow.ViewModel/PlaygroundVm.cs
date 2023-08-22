using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
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
}
