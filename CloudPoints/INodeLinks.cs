using System.Collections.Generic;

namespace CloudPoints
{
	public interface INodeLinks<TNode>
	{
		IReadOnlyCollection<EdgeLinks<TNode>> AsBegin { get; }
		IReadOnlyCollection<EdgeLinks<TNode>> AsEnd { get; }
	}
}
