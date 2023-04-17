namespace StepFlow.Layout
{
	public struct CellSize
	{
		private const string VIEW_VALUE_MESURE_SEPARATOR = " ";

		public CellSize(float value, UnitMeasure measure)
		{
			Value = value;
			Measure = measure;
		}

		public float Value { get; set; }

		public UnitMeasure Measure { get; set; }

		public override readonly string ToString() => Value.ToString() + VIEW_VALUE_MESURE_SEPARATOR + Measure.ToString();
	}
}
