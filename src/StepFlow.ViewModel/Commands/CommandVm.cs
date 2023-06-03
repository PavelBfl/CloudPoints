using System.Collections.Generic;
using StepFlow.Core.Commands;

namespace StepFlow.ViewModel.Commands
{
	public abstract class CommandVm<TCommand, TTarget, TTargetVm> : WrapperVm<TCommand>, IMarkered
		where TTarget : notnull
		where TCommand : ITargetingCommand<TTarget>
		where TTargetVm : WrapperVm<TTarget>
	{
		public CommandVm(WrapperProvider wrapperProvider, TCommand source)
			: base(wrapperProvider, source)
		{
		}

		private TTargetVm? target;

		public TTargetVm Target => target ??= WrapperProvider.GetOrCreate<TTargetVm>(Source.Target);

		public abstract bool IsMark { get; set; }

		public override IEnumerable<IWrapper> GetContent() => base.GetContent().ConcatIfNotNull(target);
	}
}
