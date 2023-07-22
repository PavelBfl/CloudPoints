namespace StepFlow.View.Sketch
{
	public struct Unit
	{
		public Unit(float value, UnitKind kind = UnitKind.Pixels)
		{
			Value = value;
			Kind = kind;
		}

		public float Value { get; set; }

		public UnitKind Kind { get; set; }
	}
}
