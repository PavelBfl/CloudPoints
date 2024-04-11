namespace StepFlow.Core.Components
{
	public sealed class Scale : ComponentBase
	{
		public static Scale Create(int max) => new Scale()
		{
			Max = max
		};

		public static Scale CreateByMax(int max) => new Scale()
		{
			Value = max,
			Max = max,
		};

		public int Value { get; set; }

		public int Max { get; set; }

		public bool Freeze { get; set; }
	}
}
