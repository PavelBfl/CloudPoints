using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public sealed class SubjectVm : WrapperVm<Subject>
	{
		public SubjectVm(LockProvider lockProvider, Subject source) : base(lockProvider, source)
		{
		}

		public bool IsSelect
		{
			get
			{
				var state = (State?)Source.Components[Master.Components.Names.STATE];

				return ((state?.Kind ?? SubjectKind.None) & SubjectKind.PlayerCharacter) != 0;
			}
		}
	}
}
