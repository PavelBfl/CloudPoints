using System.Collections.Generic;

namespace StepFlow.ViewModel.Collections
{
	public interface IReadOnlyListObservable<T> : IEnumerableObservable<T>, IReadOnlyList<T>
	{
	}
}
