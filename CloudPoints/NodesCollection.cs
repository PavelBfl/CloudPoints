using System.Collections.Generic;

namespace CloudPoints
{
	internal class NodesCollection<TNode, TEdge> : Dictionary<TNode, NodeLinks<TEdge>>, IReadOnlyDictionary<TNode, INodeLinks<TEdge>>
	{
		INodeLinks<TEdge> IReadOnlyDictionary<TNode, INodeLinks<TEdge>>.this[TNode key] => this[key];

		IEnumerable<TNode> IReadOnlyDictionary<TNode, INodeLinks<TEdge>>.Keys => Keys;

		IEnumerable<INodeLinks<TEdge>> IReadOnlyDictionary<TNode, INodeLinks<TEdge>>.Values => Values;

		IEnumerator<KeyValuePair<TNode, INodeLinks<TEdge>>> IEnumerable<KeyValuePair<TNode, INodeLinks<TEdge>>>.GetEnumerator()
		{
			foreach (var pair in this)
			{
				yield return new KeyValuePair<TNode, INodeLinks<TEdge>>(pair.Key, pair.Value);
			}
		}

		bool IReadOnlyDictionary<TNode, INodeLinks<TEdge>>.TryGetValue(TNode key, out INodeLinks<TEdge> value)
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
