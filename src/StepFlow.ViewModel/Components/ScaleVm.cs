using StepFlow.Core.Components;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Components
{
	public sealed class ScaleVm : WrapperVm<Scale>
	{
		public ScaleVm(LockProvider wrapperProvider, Scale source) : base(wrapperProvider, source)
		{
		}

		public float Value => Source.Value;

		public float Max => Source.Max;
	}
}
