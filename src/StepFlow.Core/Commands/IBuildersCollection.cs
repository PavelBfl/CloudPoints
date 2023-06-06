using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	public interface IBuildersCollection<T> : IReadOnlyDictionary<IBuilder<T>, ITargetingBuilder<T>>
	{
		bool Add(IBuilder<T> builder);
	}
}
