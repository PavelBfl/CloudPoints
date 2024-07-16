using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Place : Material
	{
		public const string PLACE_SCHEDULER = "Place";

		public Place()
		{
		}

		public Place(PlaceDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);
		}
	}
}
