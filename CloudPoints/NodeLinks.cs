using System.Collections.Generic;

namespace CloudPoints
{
	internal class NodeLinks<TEdge> : INodeLinks<TEdge>
	{
		public HashSet<TEdge> AsBegin { get; } = new HashSet<TEdge>();
		public HashSet<TEdge> AsEnd { get; } = new HashSet<TEdge>();

		IReadOnlyCollection<TEdge> INodeLinks<TEdge>.AsBegin => AsBegin;

		IReadOnlyCollection<TEdge> INodeLinks<TEdge>.AsEnd => AsEnd;
	}
}
