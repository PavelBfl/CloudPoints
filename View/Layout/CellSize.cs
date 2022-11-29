namespace StepFlow.View.Layout
{
	public struct CellSize
	{
		public CellSize(float value, UnitMeasure measure)
		{
			Value = value;
			Measure = measure;
		}

		public float Value { get; set; }

		public UnitMeasure Measure { get; set; }
	}
}
