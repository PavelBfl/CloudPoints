using StepFlow.Domains;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Place : Material
	{
		public const string PLACE_SCHEDULER = "Place";

		public Place(IContext context)
			: base(context)
		{
		}

		public Place(IContext context, PlaceDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);
		}

		public override SubjectDto ToDto()
		{
			var result = new PlaceDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(PlaceDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);
		}
	}
}
