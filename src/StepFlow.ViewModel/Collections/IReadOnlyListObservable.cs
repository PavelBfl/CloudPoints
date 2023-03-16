using System.Collections.Generic;
using System.Collections.Specialized;

namespace StepFlow.ViewModel.Collections
{
	public interface IReadOnlyListObservable<T> : IReadOnlyList<T>, INotifyCollectionChanged
	{
	}
}
