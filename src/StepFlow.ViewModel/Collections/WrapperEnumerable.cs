using System.Collections;
using System.Collections.Generic;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperEnumerable<TWrapperItem, TCollection, TModelItem> : WrapperVm<TCollection>, IEnumerable<TWrapperItem>
		where TModelItem : class
		where TWrapperItem : class, IWrapper<TModelItem>
		where TCollection : class, IEnumerable<TModelItem>
	{
		public WrapperEnumerable(LockProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public IEnumerator<TWrapperItem> GetEnumerator()
		{
			foreach (var model in Source)
			{
				yield return LockProvider.GetOrCreate<TWrapperItem>(model);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
