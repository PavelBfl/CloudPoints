using System.Collections.Generic;
using System.Linq;

namespace CloudPoints
{
	public class Graph<TNode, TEdge>
	{
		private Dictionary<TNode, NodeLinks> NodesLinks { get; } = new Dictionary<TNode, NodeLinks>();
		private Dictionary<TEdge, EdgeLinks> EdgesLinks { get; } = new Dictionary<TEdge, EdgeLinks>();

		public IReadOnlyCollection<TNode> Nodes => NodesLinks.Keys;

		public IReadOnlyCollection<TEdge> Edges => EdgesLinks.Keys;

		public INodeLinks<TEdge> GetNodeLinks(TNode node) => NodesLinks[node];

		public IEdgeLinks<TNode> GetEdgeLinks(TEdge edge) => EdgesLinks[edge];

		public void Add(TNode node)
		{
			NodesLinks.Add(node, new NodeLinks());
		}

		public bool Remove(TNode node)
		{
			if (NodesLinks.Remove(node, out var links))
			{
				foreach (var edge in links.AsBegin.Concat(links.AsEnd))
				{
					Remove(edge);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Add(TNode begin, TNode end, TEdge edge)
		{
			var beginLink = NodesLinks[begin];
			var endLink = NodesLinks[end];

			EdgesLinks.Add(edge, new EdgeLinks(begin, end));

			beginLink.AsBegin.Add(edge);
			endLink.AsEnd.Add(edge);
		}

		public bool Remove(TEdge edge)
		{
			if (EdgesLinks.Remove(edge, out var links))
			{
				NodesLinks[links.Begin].AsBegin.Remove(edge);
				NodesLinks[links.End].AsEnd.Remove(edge);
				return false;
			}
			else
			{
				return false;
			}
		}

		public void GetPath<TValue>(TNode begin, TNode end, IAccumulator<TEdge, TValue> accumulator)
		{
			
		}

		private void GetPath<TValue>(TValue length, TNode current)
		{
			
		}

		private class NodeLinks : INodeLinks<TEdge>
		{
			public HashSet<TEdge> AsBegin { get; } = new HashSet<TEdge>();
			public HashSet<TEdge> AsEnd { get; } = new HashSet<TEdge>();

			IReadOnlyCollection<TEdge> INodeLinks<TEdge>.AsBegin => AsBegin;

			IReadOnlyCollection<TEdge> INodeLinks<TEdge>.AsEnd => AsEnd;
		}

		private class EdgeLinks : IEdgeLinks<TNode>
		{
			public EdgeLinks(TNode begin, TNode end)
			{
				Begin = begin;
				End = end;
			}

			public TNode Begin { get; }
			public TNode End { get; }
		}
	}

	public interface IAccumulator<TEdge, TValue> : IComparer<TValue>
	{
		TValue Zero();

		TValue Add(TValue current, TEdge edge);
	}
}
