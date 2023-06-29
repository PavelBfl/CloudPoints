using System;
using System.Collections;
using System.Collections.Generic;
using StepFlow.Core;

namespace StepFlow.Master
{
	internal class NodeCmd : SubjectCmd<Node>, INodeCmd
	{
		public NodeCmd(PlayMaster owner, Node source) : base(owner, source)
		{
			Occupiers = new OccupiersCollection(this);
		}

		public IReadOnlyCollection<IPieceCmd> Occupiers { get; }

		private sealed class OccupiersCollection : IReadOnlyCollection<IPieceCmd>
		{
			public OccupiersCollection(NodeCmd owner) => Owner = owner ?? throw new ArgumentNullException(nameof(owner));

			private NodeCmd Owner { get; }

			public int Count => Owner.Source.Occupiers.Count;

			public IEnumerator<IPieceCmd> GetEnumerator()
			{
				foreach (var node in Owner.Source.Occupiers)
				{
					yield return new PieceCmd(Owner.Owner, node);
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
