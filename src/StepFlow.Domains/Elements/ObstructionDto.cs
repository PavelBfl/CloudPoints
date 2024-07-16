namespace StepFlow.Domains.Elements
{
	public sealed class ObstructionDto : MaterialDto
	{
		public ObstructionKind Kind { get; set; }

		public ObstructionView View { get; set; }
	}
}
