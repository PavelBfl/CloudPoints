using System.Numerics;
using StepFlow.Common;
using StepFlow.Domains;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Wormhole : Material
	{
		public Wormhole(IContext context)
			: base(context)
		{
		}

		public Wormhole(IContext context, WormholeDto original)
			: base(context, original)
		{
			Destination = PropertyCopyRequired(original.Destination, nameof(WormholeDto.Destination));
			Position = original.Position;
			Horizontal = original.Horizontal;
			Vertical = original.Vertical;
		}

		private string? destination;

		public string Destination { get => NullValidate.PropertyRequired(destination, nameof(Destination)); set => destination = NullValidate.PropertyRequired(value); }

		public Vector2 Position { get; set; }

		public HorizontalAlign Horizontal { get; set; }

		public VerticalAlign Vertical { get; set; }

		public void CopyTo(WormholeDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Destination = Destination;
			container.Position = Position;
			container.Horizontal = Horizontal;
			container.Vertical = Vertical;
		}

		public override SubjectDto ToDto()
		{
			var result = new WormholeDto();
			CopyTo(result);
			return result;
		}
	}
}
