using System.Collections.Generic;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Commands
{
	public sealed class SchedulerVm<T> : WrapperVm<IScheduler<T>>
	{
		public SchedulerVm(LockProvider wrapperProvider, IScheduler<T> source) : base(wrapperProvider, source)
		{
		}

		private BuildersCollectionVm<T>? builders;

		public BuildersCollectionVm<T> Builders => builders ??= LockProvider.GetOrCreate<BuildersCollectionVm<T>>(Source.Builders);

		private QueueVm<T>? queue;

		public QueueVm<T> Queue => queue ??= LockProvider.GetOrCreate<QueueVm<T>>(Source.Queue);

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(builders, queue);
	}
}
