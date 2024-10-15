using System.Collections.Generic;
using System.Drawing;
using StepFlow.Domains.Elements;

namespace StepFlow.Core
{
	public sealed class Context : IContext
	{
		public Intersection.Context IntersectionContext { get; } = new Intersection.Context(new Rectangle(0, 0, 100, 100));

		public Dictionary<ItemKind, ItemDto> Items { get; } = new Dictionary<ItemKind, ItemDto>();

		IReadOnlyDictionary<ItemKind, ItemDto> IContext.Items => Items;
	}
}
