namespace StepFlow.Layout
{
	public interface ICellPlot
	{
		CellPosition Position { get; }

		RectPlot Child { get; }
	}
}
