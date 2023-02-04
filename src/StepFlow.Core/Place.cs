﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public class Place : IReadOnlyDictionary<Point, HexNode>
	{
		public Place(World owner)
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

		public bool TryGetValue(Point key, out HexNode value) => Nodes.TryGetValue(key, out value);

		public IEnumerator<KeyValuePair<Point, HexNode>> GetEnumerator() => Nodes.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private World Owner { get; }

		private Dictionary<Point, HexNode> Nodes { get; } = new Dictionary<Point, HexNode>();

		public IEnumerable<Point> Keys => Nodes.Keys;

		public IEnumerable<HexNode> Values => Nodes.Values;

		public int Count => Nodes.Count;

		public HexNode this[Point key] => Nodes[key];

		public void Add(HexNode node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			Owner.Particles.Add(node);

			Nodes.Add(node.Position, node);

			OnChangeCollection();
		}

		public bool Remove(Point position)
		{
			if (TryGetValue(position, out var node))
			{
				Nodes.Remove(position);
				Owner.Particles.Remove(node);
				OnChangeCollection();
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Clear()
		{
			foreach (var node in Nodes.Values)
			{
				Owner.Particles.Remove(node);
			}

			Nodes.Clear();

			OnChangeCollection();
		}
	}
}