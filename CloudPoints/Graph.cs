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
		private NodesCollection<TNode, TEdge> NodesCollection { get; } = new NodesCollection<TNode, TEdge>();
		private EdgesCollection<TNode, TEdge> EdgesCollection { get; } = new EdgesCollection<TNode, TEdge>();

		public IReadOnlyDictionary<TNode, INodeLinks<TEdge>> Nodes => NodesCollection;

		public IReadOnlyDictionary<TEdge, IEdgeLinks<TNode>> Edges => EdgesCollection;

		public void Add(TNode node)
		{
			NodesCollection.Add(node, new NodeLinks<TEdge>());
		}

		public bool Remove(TNode node)
		{
			if (NodesCollection.Remove(node, out var links))
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
			var beginLink = NodesCollection[begin];
			var endLink = NodesCollection[end];

			EdgesCollection.Add(edge, new EdgeLinks<TNode>(begin, end));

			beginLink.AsBegin.Add(edge);
			endLink.AsEnd.Add(edge);
		}

		public bool Remove(TEdge edge)
		{
			if (EdgesCollection.Remove(edge, out var links))
			{
				NodesCollection[links.Begin].AsBegin.Remove(edge);
				NodesCollection[links.End].AsEnd.Remove(edge);
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
					GetShortestPath(pathToCurrent.Append(edge), Edges[edge].End, accumulator.Add(length, edge), accumulator, posts);
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
	}
}
