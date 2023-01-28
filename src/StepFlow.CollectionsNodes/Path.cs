using System;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.CollectionsNodes
{
	public class Path<TNode, TEdge, TLength>
	{
		public Path(TLength length, IEnumerable<object> elements)
		{
			Length = length;
			Elements = elements ?? throw new ArgumentNullException(nameof(elements));
		}

		public TLength Length { get; }

		public IEnumerable<object> Elements { get; }

		public IEnumerable<TNode> Nodes => Elements.OfType<TNode>();

		public IEnumerable<TEdge> Edges => Elements.OfType<TEdge>();
	}
}
