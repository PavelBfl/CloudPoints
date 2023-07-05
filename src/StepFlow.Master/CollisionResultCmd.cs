using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		}

		public int Count => Source.Count;

		public IReadOnlyList<ICrashCollisionCmd> Crashes { get; }

		public IReadOnlyList<IPairCollisionCmd<PairCollision>> Swaps { get; }

		public IReadOnlyList<IReadOnlyList<IPieceCmd>> Competitors => throw new System.NotImplementedException();

		public IEnumerator<IReadOnlyList<IPieceCmd>> GetEnumerator()
		{
			throw new System.NotImplementedException();
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

			protected override IPairCollisionCmd<PairCollision> Wrap(PairCollision item) => new PairCollisionCmd<PairCollision>(Owner, item);
		}
	}

	public interface IPairCollisionCmd<TSource> : IWrapperCmd<TSource>, IReadOnlyList<IPieceCmd>
		where TSource : PairCollision
	{
		
	}

	internal class PairCollisionCmd<TSource> : WrapperCmd<TSource>, IPairCollisionCmd<TSource>
		where TSource : PairCollision
	{
		public PairCollisionCmd(PlayMaster owner, TSource source) : base(owner, source)
		{
		}

		public IPieceCmd this[int index] => new PieceCmd(Owner, Source[index]);

		public int Count => Source.Count;

		public IEnumerator<IPieceCmd> GetEnumerator() => Source.Select(x => new PieceCmd(Owner, x)).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
