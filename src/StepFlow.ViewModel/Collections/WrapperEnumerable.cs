using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperEnumerable<TWrapper, TCollection, TModelItem> : WrapperVm<TCollection>, IEnumerable<TWrapper>, INotifyCollectionChanged
		where TWrapper : IWrapper<TModelItem>
		where TCollection : IEnumerable<TModelItem>
	{
		public WrapperEnumerable(LockProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);

		public IEnumerator<TWrapper> GetEnumerator()
		{
			foreach (var model in Source)
			{
				yield return LockProvider.GetOrCreate<TWrapper>(model);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override void SourceHasChange()
		{
			base.SourceHasChange();

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public override IEnumerable<ILockable> GetContent()
		{
			foreach (var content in base.GetContent())
			{
				yield return content;
			}

			foreach (var item in Source)
			{
				if (item is { } && LockProvider.TryGetValue(item, out var itemVm))
				{
					yield return itemVm;
				}
			}
		}
	}
}
