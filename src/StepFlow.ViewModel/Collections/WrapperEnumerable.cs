using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperEnumerable<TWrapper, TCollection, TModelItem> : WrapperVm<TCollection>, IEnumerable<TWrapper>, INotifyCollectionChanged
		where TWrapper : IWrapper
		where TCollection : IEnumerable<TModelItem>
	{
		public WrapperEnumerable(WrapperProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);

		public IEnumerator<TWrapper> GetEnumerator()
		{
			foreach (var model in Source)
			{
				yield return WrapperProvider.GetOrCreate<TWrapper>(model);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override void Refresh()
		{
			base.Refresh();

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public override IEnumerable<IWrapper> GetContent()
		{
			foreach (var content in base.GetContent())
			{
				yield return content;
			}

			foreach (var item in Source)
			{
				if (item is { } && WrapperProvider.TryGetValue(item, out var itemVm))
				{
					yield return itemVm;
				}
			}
		}
	}
}
