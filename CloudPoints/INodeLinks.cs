using System.Collections.Generic;

namespace StepFlow.CollectionsNodes
{
	public interface INodeLinks<TNode>
	{
		IReadOnlyCollection<EdgeLinks<TNode>> AsBegin { get; }
		IReadOnlyCollection<EdgeLinks<TNode>> AsEnd { get; }
	}
}
