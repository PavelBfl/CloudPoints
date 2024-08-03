using StepFlow.Domains;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class PlaygroundSwitch : Material
	{
		public PlaygroundSwitch(IContext context)
			: base(context)
		{
		}

		public PlaygroundSwitch(IContext context, PlaygroundSwitchDto original)
			: base(context, original)
		{
			Destination = PropertyCopyRequired(original.Destination, nameof(PlaygroundSwitchDto.Destination));
		}

		private string? destination;

		public string Destination { get => PropertyRequired(destination); set => destination = PropertyRequired(value); }

		public void CopyTo(PlaygroundSwitchDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Destination = Destination;
		}

		public override SubjectDto ToDto()
		{
			var result = new PlaygroundSwitchDto();
			CopyTo(result);
			return result;
		}
	}
}
