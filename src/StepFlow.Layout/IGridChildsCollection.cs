using System.Collections.Generic;
using System.Collections.Specialized;

namespace StepFlow.Layout
{
	public interface IGridChildsCollection : IReadOnlyList<ICellPlot>, INotifyCollectionChanged
	{
		void Add(RectPlot plot, CellPosition position);
		void Remove(RectPlot plot);
	}
}
