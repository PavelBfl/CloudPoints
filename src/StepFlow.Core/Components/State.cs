namespace StepFlow.Core.Components
{
	public sealed class State : ComponentBase
	{
		public State(Playground owner) : base(owner)
		{
		}

		public int Team { get; set; }
	}
}
