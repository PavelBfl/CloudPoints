using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	internal sealed class PlaceCmd : IPlaceCmd
	{
		public PlaceCmd(PlayMaster owner, Place place)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Place = place ?? throw new ArgumentNullException(nameof(place));
		}

		public INodeCmd this[Point key] => CreateNode(Place[key]);

		public IEnumerable<Point> Keys => Place.Keys;

		public IEnumerable<INodeCmd> Values => Place.Values.Select(CreateNode);

		public int Count => Place.Count;

		private PlayMaster Owner { get; }

		private Place Place { get; }

		private INodeCmd CreateNode(Node node) => new NodeCmd(Owner, node);

		public void Add(INodeCmd node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			Owner.TimeAxis.Add(new PlaceAdd(Place, node.Source));
		}

		public bool Remove(Point position)
		{
			if (Place.TryGetValue(position, out var node))
			{
				Owner.TimeAxis.Add(new Reverse(new PlaceAdd(Place, node)));
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ContainsKey(Point key) => Place.ContainsKey(key);

		public IEnumerator<KeyValuePair<Point, INodeCmd>> GetEnumerator()
			=> Place.Select(x => KeyValuePair.Create(x.Key, CreateNode(x.Value))).GetEnumerator();

		public bool TryGetValue(Point key, [MaybeNullWhen(false)] out INodeCmd value)
		{
			if (Place.TryGetValue(key, out var node))
			{
				value = CreateNode(node);
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
