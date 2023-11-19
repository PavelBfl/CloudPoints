namespace StepFlow.Core.Components
{
	public sealed class Scale : Subject
	{
		public int Value { get; set; }

		public int Max { get; set; }

		public bool Freeze { get; set; }
	}
}
