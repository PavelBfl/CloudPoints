using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public sealed class PlaceVm : WrapperVm<Place<GamePlay.Node>>
	{
		public PlaceVm(WrapperProvider wrapperProvider, Place<GamePlay.Node> items)
			: base(wrapperProvider, items)
		{
		}
	}
}
