using System;

namespace CloudPoints
{
	public readonly struct EdgeLinks<TNode>
	{
		public EdgeLinks(TNode begin, TNode end)
		{
			Begin = begin;
			End = end;
		}

		public TNode Begin { get; }
		public TNode End { get; }
	}
}
