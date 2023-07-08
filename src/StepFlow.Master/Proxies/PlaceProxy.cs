using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies
{
	internal sealed class PlaceProxy : ProxyBase<Place>, IReadOnlyDictionary<Point, Node>
	{
		[MoonSharpHidden]
		public PlaceProxy(PlayMaster owner, Place target) : base(owner, target)
		{
		}

		public Node this[Point key] => Target[key];

		public Node this[int x, int y] => this[new Point(x, y)];

		public IEnumerable<Point> Keys => Target.Keys;

		public IEnumerable<Node> Values => Target.Values;

		public int Count => Target.Count;

		public void Add(Node node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			Owner.TimeAxis.Add(new PlaceAdd(Target, node));
		}

		public bool Remove(Point position)
		{
			if (Target.TryGetValue(position, out var node))
			{
				Owner.TimeAxis.Add(new Reverse(new PlaceAdd(Target, node)));
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ContainsKey(Point key) => Target.ContainsKey(key);

		public IEnumerator<KeyValuePair<Point, Node>> GetEnumerator() => Target.GetEnumerator();

		public bool TryGetValue(Point key, out Node value) => Target.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
