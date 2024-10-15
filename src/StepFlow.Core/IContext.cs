using System.Collections.Generic;
using StepFlow.Domains.Elements;

namespace StepFlow.Core
{
	public interface IContext
	{
		Intersection.Context IntersectionContext { get; }

		IReadOnlyDictionary<ItemKind, ItemDto> Items { get; }
	}
}
