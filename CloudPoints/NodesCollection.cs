using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CloudPoints
{
	internal class NodesCollection<TNode> : Dictionary<TNode, NodeLinks<TNode>>, IReadOnlyDictionary<TNode, INodeLinks<TNode>>
	{
		public NodesCollection(IEqualityComparer<TNode> comparer)
			: base(comparer)
		{
		}

		INodeLinks<TNode> IReadOnlyDictionary<TNode, INodeLinks<TNode>>.this[TNode key] => this[key];

		IEnumerable<TNode> IReadOnlyDictionary<TNode, INodeLinks<TNode>>.Keys => Keys;

		IEnumerable<INodeLinks<TNode>> IReadOnlyDictionary<TNode, INodeLinks<TNode>>.Values => Values;

		IEnumerator<KeyValuePair<TNode, INodeLinks<TNode>>> IEnumerable<KeyValuePair<TNode, INodeLinks<TNode>>>.GetEnumerator()
		{
			foreach (var pair in this)
			{
				yield return new KeyValuePair<TNode, INodeLinks<TNode>>(pair.Key, pair.Value);
			}
		}

		bool IReadOnlyDictionary<TNode, INodeLinks<TNode>>.TryGetValue(TNode key, out INodeLinks<TNode> value)
		{
			if (TryGetValue(key, out var links))
			{
				value = links;
				return true;
			}
			else
			{
				value = default!;
				return false;
			}
		}
	}
}
