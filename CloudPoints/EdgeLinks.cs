namespace CloudPoints
{
	internal class EdgeLinks<TNode> : IEdgeLinks<TNode>
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
