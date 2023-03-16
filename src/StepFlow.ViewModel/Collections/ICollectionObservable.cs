using System.Collections.Generic;

namespace StepFlow.ViewModel.Collections
{
	public interface ICollectionObservable<T> : IReadOnlyListObservable<T>, ICollection<T>
	{
	}
}
