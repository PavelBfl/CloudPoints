using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	internal sealed class PlaygroundCmd : ContainerCmd<Playground>, IPlaygroundCmd
	{
		public PlaygroundCmd(PlayMaster owner, Playground source) : base(owner, source)
		{
			Pieces = new PiecesCollectionCmd(this);
			Place = new PlaceCmd(Owner, Source.Place);
		}

		public ICollectionCmd<IPieceCmd> Pieces { get; }

		public IPlaceCmd Place { get; }

		public ICollisionResultCmd GetCollision() => new CollisionResultCmd(Owner, Source.GetCollision());

		public INodeCmd CreateNode(int x, int y) => new NodeCmd(Owner, new Node(Source, new Point(x, y)));

		public IPieceCmd CreatePiece() => new PieceCmd(Owner, new Piece(Source));

		private sealed class PiecesCollectionCmd : ICollectionCmd<IPieceCmd>
		{
			public PiecesCollectionCmd(PlaygroundCmd owner)
				=> Owner = owner ?? throw new ArgumentNullException(nameof(owner));

			public int Count => Owner.Source.Pieces.Count;

			public bool IsReadOnly => false;

			private PlaygroundCmd Owner { get; }

			public void Add(IPieceCmd item) => Owner.Owner.TimeAxis.Add(new AddItemCommand<Piece>(Owner.Source.Pieces, item.Source));

			public void Clear() => Owner.Owner.TimeAxis.Add(new ClearCommand<Piece>(Owner.Source.Pieces));

			public bool Contains(IPieceCmd item) => Owner.Source.Pieces.Contains(item.Source);

			public void CopyTo(IPieceCmd[] array, int arrayIndex)
			{
				foreach (var piece in this)
				{
					array[arrayIndex] = piece;
					arrayIndex++;
				}
			}

			public IEnumerator<IPieceCmd> GetEnumerator()
				=> Owner.Source.Pieces.Select(x => new PieceCmd(Owner.Owner, x)).GetEnumerator();

			public bool Remove(IPieceCmd item)
			{
				var removed = Owner.Source.Pieces.Remove(item.Source);
				if (removed)
				{
					Owner.Owner.TimeAxis.Add(new Reverse(new AddItemCommand<Piece>(Owner.Source.Pieces, item.Source)), true);
				}

				return removed;
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
