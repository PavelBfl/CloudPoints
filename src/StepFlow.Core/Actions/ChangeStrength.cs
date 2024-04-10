using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Core.Actions
{
	public sealed class ChangeStrength : ActionBase
	{
		public Material? Material { get; set; }

		public Damage Damage { get; set; }
	}
}
