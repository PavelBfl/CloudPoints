using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Commands
{
	public sealed class QueueVm<T> : WrapperVm<IQueue<T>>, IEnumerableObservable<KeyValuePair<long, IReadOnlyCollection<ITargetingCommand<T>>>>
	{
		internal QueueVm(LockProvider wrapperProvider, IQueue<T> source) : base(wrapperProvider, source)
		{
		}

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public override void SourceHasChange()
		{
			base.SourceHasChange();

			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public IEnumerator<KeyValuePair<long, IReadOnlyCollection<ITargetingCommand<T>>>> GetEnumerator() => Source.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
