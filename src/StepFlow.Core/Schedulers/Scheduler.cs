using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Core.Schedulers
{
	public class Scheduler : ElementBase
	{
		public Turn? Current { get; set; }
	}
}
