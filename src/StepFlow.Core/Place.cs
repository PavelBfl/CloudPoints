using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public sealed class Place : IReadOnlyDictionary<Point, Node>
	{
		public Place(Playground owner)
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

		public bool TryGetValue(Point key, out Node value) => Nodes.TryGetValue(key, out value);

		public IEnumerator<KeyValuePair<Point, Node>> GetEnumerator() => Nodes.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private Playground Owner { get; }

		private Dictionary<Point, Node> Nodes { get; } = new Dictionary<Point, Node>();

		public IEnumerable<Point> Keys => Nodes.Keys;

		public IEnumerable<Node> Values => Nodes.Values;

		public int Count => Nodes.Count;

		public Node this[Point key] => Nodes[key];

		public void Add(Node node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			Nodes.Add(node.Position, node);
		}

		public bool Remove(Point position) => Nodes.Remove(position);

		public void Clear()
		{
			Nodes.Clear();

			OnChangeCollection();
		}
	}
}
