using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Commands
{
	public sealed class BuildersCollectionVm<T> : WrapperVm<IBuildersCollection<T>>, IEnumerableObservable<KeyValuePair<IBuilder<T>, ITargetingBuilder<T>>>
	{
		internal BuildersCollectionVm(LockProvider wrapperProvider, IBuildersCollection<T> source) : base(wrapperProvider, source)
		{
		}

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public override void SourceHasChange()
		{
			base.SourceHasChange();

			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public IEnumerator<KeyValuePair<IBuilder<T>, ITargetingBuilder<T>>> GetEnumerator() => Source.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
