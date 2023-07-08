using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies
{
	public sealed class PiecesCollectionProxy : ProxyBase<PiecesCollection>, ICollection<Piece>
	{
		[MoonSharpHidden]
		public PiecesCollectionProxy(PlayMaster owner, PiecesCollection target) : base(owner, target)
		{
		}

		public int Count => Target.Count;

		public bool IsReadOnly => false;

		public void Add(Piece item) => Owner.TimeAxis.Add(new AddItemCommand<Piece>(Target, item));

		public void Clear() => Owner.TimeAxis.Add(new ClearCommand<Piece>(Target));

		public bool Contains(Piece item) => Target.Contains(item);

		public void CopyTo(Piece[] array, int arrayIndex) => Target.CopyTo(array, arrayIndex);

		public IEnumerator<Piece> GetEnumerator() => Target.GetEnumerator();

		public bool Remove(Piece item)
		{
			var removed = Target.Remove(item);
			if (removed)
			{
				Owner.TimeAxis.Add(new Reverse(new AddItemCommand<Piece>(Target, item)), true);
			}

			return removed;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
