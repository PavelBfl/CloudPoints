namespace StepFlow.Core.Components
{
	public sealed class Action
	{
		public long Begin { get; set; }

		public long Duration { get; set; }

		public Subject? Executor { get; set; }
	}
}
