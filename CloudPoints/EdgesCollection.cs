using System.Collections.Generic;

namespace CloudPoints
{
	internal class EdgesCollection<TNode, TEdge> : Dictionary<TEdge, EdgeLinks<TNode>>, IReadOnlyDictionary<TEdge, IEdgeLinks<TNode>>
	{
		IEdgeLinks<TNode> IReadOnlyDictionary<TEdge, IEdgeLinks<TNode>>.this[TEdge key] => this[key];

		IEnumerable<TEdge> IReadOnlyDictionary<TEdge, IEdgeLinks<TNode>>.Keys => Keys;

		IEnumerable<IEdgeLinks<TNode>> IReadOnlyDictionary<TEdge, IEdgeLinks<TNode>>.Values => Values;

		IEnumerator<KeyValuePair<TEdge, IEdgeLinks<TNode>>> IEnumerable<KeyValuePair<TEdge, IEdgeLinks<TNode>>>.GetEnumerator()
		{
			foreach (var pair in this)
			{
				yield return new KeyValuePair<TEdge, IEdgeLinks<TNode>>(pair.Key, pair.Value);
			}
		}

		bool IReadOnlyDictionary<TEdge, IEdgeLinks<TNode>>.TryGetValue(TEdge key, out IEdgeLinks<TNode> value)
		{
			if (TryGetValue(key, out var links))
			{
				value = links;
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}
	}
}
