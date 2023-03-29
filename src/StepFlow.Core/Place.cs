using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public sealed class Place<TNode> : IReadOnlyDictionary<Point, TNode>
		where TNode : Node
	{
		public Place(IWorld owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		private Rectangle? bounds;
		public Rectangle Bounds => bounds ??= GetBounds();

		private void OnChangeCollection() => bounds = null;

		private Rectangle GetBounds()
		{
			if (this.Any())
			{
				var left = int.MaxValue;
				var right = int.MinValue;
				var top = int.MinValue;
				var bottom = int.MaxValue;

				foreach (var point in Keys)
				{
					left = Math.Min(left, point.X);
					right = Math.Max(right, point.X);
					top = Math.Max(top, point.Y);
					bottom = Math.Min(bottom, point.Y);
				}

				return Rectangle.FromLTRB(left, top, right, bottom);
			}
			else
			{
				return Rectangle.Empty;
			}
		}

		public bool ContainsKey(Point key) => Nodes.ContainsKey(key);

		public bool TryGetValue(Point key, out TNode value) => Nodes.TryGetValue(key, out value);

		public IEnumerator<KeyValuePair<Point, TNode>> GetEnumerator() => Nodes.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private IWorld Owner { get; }

		private Dictionary<Point, TNode> Nodes { get; } = new Dictionary<Point, TNode>();

		public IEnumerable<Point> Keys => Nodes.Keys;

		public IEnumerable<TNode> Values => Nodes.Values;

		public int Count => Nodes.Count;

		public TNode this[Point key] => Nodes[key];

		public void Add(TNode node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			node.Owner = Owner;

			Nodes.Add(node.Position, node);
		}

		public bool Remove(Point position)
		{
			if (TryGetValue(position, out var node))
			{
				node.Owner = null;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Clear()
		{
			foreach (var node in Nodes.Values.ToArray())
			{
				node.Owner = null;
			}

			OnChangeCollection();
		}
	}
}
