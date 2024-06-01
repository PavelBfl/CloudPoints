namespace StepFlow.View.Sketch
{
	public struct Thickness
	{
		public Thickness(float all, UnitKind kind = UnitKind.Pixels)
			: this(all, all, all, all, kind)
		{
		}

		public Thickness(float left, float top, float right, float bottom, UnitKind kind = UnitKind.Pixels)
		{
			Left = new Unit(left, kind);
			Top = new Unit(top, kind);
			Right = new Unit(right, kind);
			Bottom = new Unit(bottom, kind);
		}

		public Unit Left { get; set; }

		public Unit Top { get; set; }

		public Unit Right { get; set; }

		public Unit Bottom { get; set; }
	}
}
