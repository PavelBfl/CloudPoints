using System.Collections.Generic;

namespace StepFlow.ViewModel.Collections
{
	public interface IListObservable<T> : ICollectionObservable<T>, IList<T>
	{
	}
}
