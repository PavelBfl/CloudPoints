using System.Collections.Generic;

namespace StepFlow.Master
{
	public interface ICollectionCmd<T> : ICollection<T>, IReadOnlyCollection<T>
	{
		
	}
}
