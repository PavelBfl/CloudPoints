using System.Collections.Generic;

namespace CloudPoints
{
	public interface IAccumulator<TEdge, TValue> : IComparer<TValue>
	{
		TValue Zero();

		TValue Invinite();

		TValue Add(TValue current, TEdge edge);
	}
}
