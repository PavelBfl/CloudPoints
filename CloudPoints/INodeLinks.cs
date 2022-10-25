using System.Collections.Generic;

namespace CloudPoints
{
	public interface INodeLinks<out TEdge>
	{
		IReadOnlyCollection<TEdge> AsBegin { get; }
		IReadOnlyCollection<TEdge> AsEnd { get; }
	}
}
