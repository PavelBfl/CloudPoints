using System.Collections.Generic;
using System.Linq;

namespace StepFlow.ViewModel.Collector
{
	public static class LockableExtensions
	{
		public static IEnumerable<ILockable> ConcatIfNotNull(this IEnumerable<ILockable> container, params ILockable?[] wrappers)
		{
			var wrappersRequired = from wrapper in wrappers
								   where wrapper is { }
								   select wrapper;

			return container.Concat(wrappersRequired);
		}
	}
}
