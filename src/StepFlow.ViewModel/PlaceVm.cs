using System;
using System.Collections.Generic;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

namespace StepFlow.ViewModel
{
	public sealed class PlaceVm : WrapperObserver<NodeVm, Node>
	{
		public PlaceVm(ContextVm owner, IEnumerable<Node> items)
			: base(items)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		private ContextVm Owner { get; }

		protected override NodeVm CreateObserver(Node observable)
		{
			if (Owner.WrapperProvider.TryGetViewModel(observable, out object result))
			{
				return (NodeVm)result;
			}
			else
			{
				return new NodeVm(Owner, observable);
			}
		}
	}
}
