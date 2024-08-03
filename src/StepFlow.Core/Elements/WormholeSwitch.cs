using System.Numerics;
using StepFlow.Domains;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class WormholeSwitch : Material
	{
		public WormholeSwitch(IContext context)
			: base(context)
		{
		}

		public WormholeSwitch(IContext context, WormholeDto original)
			: base(context, original)
		{
			Destination = PropertyCopyRequired(original.Destination, nameof(WormholeDto.Destination));
			Position = original.Position;
		}

		private string? destination;

		public string Destination { get => PropertyRequired(destination); set => destination = PropertyRequired(value); }

		public Vector2 Position { get; set; }

		public void CopyTo(WormholeDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Destination = Destination;
			container.Position = Position;
		}

		public override SubjectDto ToDto()
		{
			var result = new WormholeDto();
			CopyTo(result);
			return result;
		}
	}
}
