using System;
using System.Collections.Generic;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

namespace StepFlow.ViewModel
{
	public sealed class PlaceVm : WrapperObserver<NodeVm, Node>
	{
		public PlaceVm(IEnumerable<Node> items)
			: base(items)
		{
		}

		protected override NodeVm CreateObserver(Node observable) => WrapperProvider.GetOrCreate<NodeVm>(observable);
	}
}
