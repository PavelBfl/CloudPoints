namespace CloudPoints
{
	public interface IEdgeLinks<out TNode>
	{
		TNode Begin { get; }
		TNode End { get; }
	}
}
