namespace StepFlow.Core.Components
{
	public class Handler : ComponentBase
	{
		public Handler(Playground owner) : base(owner)
		{
		}

		public bool Disposable { get; set; }

		public string? Reference { get; set; }
	}
}
