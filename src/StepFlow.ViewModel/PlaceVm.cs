using System;
using System.Collections.Generic;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

namespace StepFlow.ViewModel
{
	public sealed class PlaceVm : WrapperObserver<NodeVm, Node>
	{
		public PlaceVm(WrapperProvider wrapperProvider, IEnumerable<Node> items)
			: base(items)
		{
			WrapperProvider = wrapperProvider ?? throw new ArgumentNullException(nameof(wrapperProvider));
		}

		private WrapperProvider WrapperProvider { get; }

		protected override NodeVm CreateObserver(Node observable) => WrapperProvider.GetOrCreate<NodeVm>(observable);
	}
}
