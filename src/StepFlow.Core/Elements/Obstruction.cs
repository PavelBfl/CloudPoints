using StepFlow.Domains;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Obstruction : Material
	{
		public Obstruction(IContext context)
			: base(context)
		{
		}

		public Obstruction(IContext context, ObstructionDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Kind = original.Kind;
			View = original.View;
		}

		public ObstructionKind Kind { get; set; }

		public ObstructionView View { get; set; }

		public override SubjectDto ToDto()
		{
			var result = new ObstructionDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(ObstructionDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Kind = Kind;
			container.View = View;
		}
	}
}
