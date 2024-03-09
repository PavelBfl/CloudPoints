using StepFlow.Core.Elements;

namespace StepFlow.Core.Components
{
	public sealed class ChangeStrength : ComponentBase
	{
		public Material? Material { get; set; }

		public Damage Damage { get; set; }
	}
}
