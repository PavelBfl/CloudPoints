using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CloudPoints
{
	public class Graph<TNode, TEdge>
		where TNode : notnull
		where TEdge : notnull
	{
		public Graph(IEqualityComparer<TNode>? nodeComparer = null)
		{
			LinkComparer = new EdgeLinkComparer(nodeComparer ?? EqualityComparer<TNode>.Default);
			NodesCollection = new NodesCollection<TNode>(NodeComparer);
			EdgesCollection = new Dictionary<EdgeLinks<TNode>, TEdge>(LinkComparer);
		}

		private NodesCollection<TNode> NodesCollection { get; }
		private Dictionary<EdgeLinks<TNode>, TEdge> EdgesCollection { get; }

		public IReadOnlyDictionary<TNode, INodeLinks<TNode>> Nodes => NodesCollection;

		public IReadOnlyDictionary<EdgeLinks<TNode>, TEdge> Edges => EdgesCollection;

		private EdgeLinkComparer LinkComparer { get; }
		public IEqualityComparer<TNode> NodeComparer => LinkComparer.Source;

		public void Add(TNode node)
		{
			NodesCollection.Add(node, new NodeLinks<TNode>(LinkComparer));
		}

		public bool Remove(TNode node)
		{
			if (NodesCollection.Remove(node, out var links))
			{
				foreach (var edge in links.AsBegin.Concat(links.AsEnd))
				{
					Remove(edge.Begin, edge.End);
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
			var beginLink = NodesCollection[begin];
			var endLink = NodesCollection[end];

			var edgeLink = new EdgeLinks<TNode>(begin, end);
			EdgesCollection.Add(edgeLink, edge);

			beginLink.AsBegin.Add(edgeLink);
			endLink.AsEnd.Add(edgeLink);
		}

		public bool Remove(TNode begin, TNode end)
		{
			var edgeLink = new EdgeLinks<TNode>(begin, end);
			if (EdgesCollection.Remove(edgeLink))
			{
				NodesCollection[begin].AsBegin.Remove(edgeLink);
				NodesCollection[end].AsEnd.Remove(edgeLink);
				return false;
			}
			else
			{
				return false;
			}
		}

		public IEnumerable<Path<TNode, TEdge, TLength>> GetShortestPath<TLength>(TNode begin, TNode end, IAccumulator<TEdge, TLength> accumulator)
		{
			var posts = new Dictionary<TNode, Post<TLength>>();
			foreach (var node in Nodes.Keys)
			{
				posts.Add(node, new Post<TLength>(accumulator));
			}

			GetShortestPath(Enumerable.Empty<object>(), begin, accumulator.Zero(), accumulator, posts);

			var post = posts[end];
			foreach (var path in post.Paths)
			{
				yield return new Path<TNode, TEdge, TLength>(post.Length, path);
			}
		}

		private void GetShortestPath<TLength>(IEnumerable<object> path, TNode current, TLength length, IAccumulator<TEdge, TLength> accumulator, Dictionary<TNode, Post<TLength>> posts)
		{
			var pathToCurrent = path.Append(current);
			if (posts[current].TrySetLength(pathToCurrent, length))
			{
				foreach (var edge in Nodes[current].AsBegin)
				{
					GetShortestPath(pathToCurrent.Append(edge), edge.End, accumulator.Add(length, Edges[edge]), accumulator, posts);
				}
			}
		}

		private class Post<TLength>
		{
			public Post(IAccumulator<TEdge, TLength> accumulator)
			{
				Accumulator = accumulator ?? throw new ArgumentNullException(nameof(accumulator));
				Length = Accumulator.Invinite();
			}

			private IAccumulator<TEdge, TLength> Accumulator { get; }

			public TLength Length { get; private set; }

			public ICollection<IEnumerable<object>> Paths { get; } = new List<IEnumerable<object>>();

			public bool TrySetLength(IEnumerable<object> path, TLength newLength)
			{
				if (path is null)
				{
					throw new ArgumentNullException(nameof(path));
				}

				var compare = Accumulator.Compare(newLength, Length);

				if (compare < 0)
				{
					return SetLength(path, newLength, true);
				}
				else if (compare == 0)
				{
					return SetLength(path, newLength, false);
				}
				else
				{
					return false;
				}
			}

			private bool SetLength(IEnumerable<object> path, TLength newLength, bool clear)
			{
				if (clear)
				{
					Paths.Clear();
				}
				Paths.Add(path);
				Length = newLength;
				return true;
			}
		}

		private sealed class EdgeLinkComparer : IEqualityComparer<EdgeLinks<TNode>>
		{
			public EdgeLinkComparer(IEqualityComparer<TNode> source) => Source = source ?? throw new ArgumentNullException(nameof(source));

			public IEqualityComparer<TNode> Source { get; }

			public bool Equals(EdgeLinks<TNode> x, EdgeLinks<TNode> y) => Source.Equals(x.Begin, y.Begin) && Source.Equals(x.End, y.End);

			public int GetHashCode(EdgeLinks<TNode> obj)
			{
				var result = default(HashCode);
				result.Add(obj.Begin, Source);
				result.Add(obj.End, Source);
				return result.ToHashCode();
			}
		}
	}
}
