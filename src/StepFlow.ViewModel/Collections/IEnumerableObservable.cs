using System.Collections.Generic;
using System.Collections.Specialized;

namespace StepFlow.ViewModel.Collections
{
	public interface IEnumerableObservable<T> : IEnumerable<T>, INotifyCollectionChanged
	{
	}
}
