using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StepFlow.Core.Commands;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Commands
{
	public interface ICommandVm<out T> : IWrapper<T>
		where T : notnull, ICommand
	{
		
	}

	public abstract class CommandVm<TCommand, TTarget, TTargetVm> : WrapperVm<TCommand>, ICommandVm<TCommand>, IMarkered
		where TCommand : ITargetingCommand<TTarget>
		where TTarget : notnull
		where TTargetVm : IWrapper<TTarget>
	{
		public CommandVm(LockProvider wrapperProvider, TCommand source)
			: base(wrapperProvider, source)
		{
		}

		[MaybeNull]
		[AllowNull]
		private TTargetVm target;

		public TTargetVm Target => target ??= LockProvider.GetOrCreate<TTargetVm>(Source.Target);

		public abstract bool IsMark { get; set; }

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(target);
	}
}
