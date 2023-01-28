using System.Collections.Generic;

namespace StepFlow.CollectionsNodes
{
	internal class NodeLinks<TNode> : INodeLinks<TNode>
	{
		public NodeLinks(IEqualityComparer<EdgeLinks<TNode>> equalityComparer)
		{
			AsBegin = new HashSet<EdgeLinks<TNode>>(equalityComparer);
			AsEnd = new HashSet<EdgeLinks<TNode>>(equalityComparer);
		}

		public HashSet<EdgeLinks<TNode>> AsBegin { get; }

		public HashSet<EdgeLinks<TNode>> AsEnd { get; }

		IReadOnlyCollection<EdgeLinks<TNode>> INodeLinks<TNode>.AsBegin => AsBegin;

		IReadOnlyCollection<EdgeLinks<TNode>> INodeLinks<TNode>.AsEnd => AsEnd;
	}
}
