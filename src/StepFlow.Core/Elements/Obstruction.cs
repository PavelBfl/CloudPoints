using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Obstruction : Material
	{
		public Obstruction()
		{
		}

		public Obstruction(ObstructionDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);

			Kind = original.Kind;
			View = original.View;
		}

		public ObstructionKind Kind { get; set; }

		public ObstructionView View { get; set; }
	}
}
