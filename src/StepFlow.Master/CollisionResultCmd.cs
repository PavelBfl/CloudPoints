using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Collision;

namespace StepFlow.Master
{
	public interface IReadOnlyCollectionCmd<TSource, TItem, TWrapItem> : IWrapperCmd<TSource>, IReadOnlyCollection<TWrapItem>
		where TSource : IReadOnlyCollection<TItem>
	{
	}

	internal abstract class ReadOnlyCollectionCmd<TSource, TItem, TWrapItem> : WrapperCmd<TSource>, IReadOnlyCollectionCmd<TSource, TItem, TWrapItem>
		where TSource : class, IReadOnlyCollection<TItem>
	{
		protected ReadOnlyCollectionCmd(PlayMaster owner, TSource source) : base(owner, source)
		{
		}

		public int Count => Source.Count;

		public IEnumerator<TWrapItem> GetEnumerator() => Source.Select(Wrap).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		protected abstract TWrapItem Wrap(TItem item);
	}

	public interface IReadOnlyListCmd<TSource, TItem, TWrapItem> : IReadOnlyCollectionCmd<TSource, TItem, TWrapItem>, IReadOnlyList<TWrapItem>
		where TSource : IReadOnlyList<TItem>
	{
	}

	internal abstract class ReadOnlyListCmd<TSource, TItem, TWrapItem> : ReadOnlyCollectionCmd<TSource, TItem, TWrapItem>, IReadOnlyListCmd<TSource, TItem, TWrapItem>
		where TSource : class, IReadOnlyList<TItem>
	{
		protected ReadOnlyListCmd(PlayMaster owner, TSource source) : base(owner, source)
		{
		}

		public TWrapItem this[int index] => Wrap(Source[index]);
	}

	public interface ICollisionResultCmd : IWrapperCmd<CollisionResult>, IReadOnlyCollection<IReadOnlyList<IPieceCmd>>
	{
		IReadOnlyList<ICrashCollisionCmd> Crashes { get; }

		IReadOnlyList<IPairCollisionCmd<PairCollision>> Swaps { get; }

		IReadOnlyList<IReadOnlyList<IPieceCmd>> Competitors { get; }
	}

	internal sealed class CollisionResultCmd : WrapperCmd<CollisionResult>, ICollisionResultCmd
	{
		public CollisionResultCmd(PlayMaster owner, CollisionResult source) : base(owner, source)
		{
			Crashes = new CrashesListCmd(Owner, Source.Crashes);
			Swaps = new SwapsListCmd(Owner, Source.Swaps);
			Competitors = new CompetitorsCollection(Owner, Source.Competitors);
		}

		public int Count => Source.Count;

		public IReadOnlyList<ICrashCollisionCmd> Crashes { get; }

		public IReadOnlyList<IPairCollisionCmd<PairCollision>> Swaps { get; }

		public IReadOnlyList<IReadOnlyList<IPieceCmd>> Competitors { get; }

		public IEnumerator<IReadOnlyList<IPieceCmd>> GetEnumerator()
		{
			// Так сложно сделано с инстанцированием коллекции в Array т.к. lua у Enumerator может вызвать Reset который не поддерживает yield return
			return Source.Select(x => new PiecesReadOnlyListCmd<IReadOnlyList<Piece>>(Owner, x))
				.ToArray()
				.AsEnumerable<IReadOnlyList<IPieceCmd>>()
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class CrashesListCmd : ReadOnlyListCmd<IReadOnlyList<CrashCollision>, CrashCollision, ICrashCollisionCmd>
		{
			public CrashesListCmd(PlayMaster owner, IReadOnlyList<CrashCollision> source) : base(owner, source)
			{
			}

			protected override ICrashCollisionCmd Wrap(CrashCollision item) => new CrashCollisionCmd(Owner, item);
		}

		private sealed class SwapsListCmd : ReadOnlyListCmd<IReadOnlyList<PairCollision>, PairCollision, IPairCollisionCmd<PairCollision>>
		{
			public SwapsListCmd(PlayMaster owner, IReadOnlyList<PairCollision> source) : base(owner, source)
			{
			}

			protected override IPairCollisionCmd<PairCollision> Wrap(PairCollision item)
				=> new PairCollisionCmd<PairCollision>(Owner, item);
		}

		private sealed class CompetitorsCollection : ReadOnlyListCmd<
			IReadOnlyList<IReadOnlyList<Piece>>,
			IReadOnlyList<Piece>,
			IReadOnlyListCmd<IReadOnlyList<Piece>, Piece, IPieceCmd>
		>
		{
			public CompetitorsCollection(PlayMaster owner, IReadOnlyList<IReadOnlyList<Piece>> source) : base(owner, source)
			{
			}

			protected override IReadOnlyListCmd<IReadOnlyList<Piece>, Piece, IPieceCmd> Wrap(IReadOnlyList<Piece> item)
				=> new PiecesReadOnlyListCmd<IReadOnlyList<Piece>>(Owner, item);
		}
	}

	public interface IPairCollisionCmd<TSource> : IWrapperCmd<TSource>, IReadOnlyList<IPieceCmd>
		where TSource : PairCollision
	{
		
	}

	internal class PiecesReadOnlyListCmd<TSource> : ReadOnlyListCmd<TSource, Piece, IPieceCmd>
		where TSource : class, IReadOnlyList<Piece>
	{
		public PiecesReadOnlyListCmd(PlayMaster owner, TSource source) : base(owner, source)
		{
		}

		protected override IPieceCmd Wrap(Piece item) => new PieceCmd(Owner, item);
	}

	internal class PairCollisionCmd<TSource> : PiecesReadOnlyListCmd<TSource>, IPairCollisionCmd<TSource>
		where TSource : PairCollision
	{
		public PairCollisionCmd(PlayMaster owner, TSource source) : base(owner, source)
		{
		}

		protected override IPieceCmd Wrap(Piece item) => new PieceCmd(Owner, item);
	}

	internal sealed class CrashCollisionCmd : PairCollisionCmd<CrashCollision>, ICrashCollisionCmd
	{
		public CrashCollisionCmd(PlayMaster owner, CrashCollision source) : base(owner, source)
		{
		}

		public IPieceCmd Stationary => new PieceCmd(Owner, Source.Stationary);

		public IPieceCmd Moved => new PieceCmd(Owner, Source.Moved);
	}

	public interface ICrashCollisionCmd : IPairCollisionCmd<CrashCollision>
	{
		IPieceCmd Stationary { get; }

		IPieceCmd Moved { get; }
	}
}
