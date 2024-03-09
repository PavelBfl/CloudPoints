using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerDamage : Scheduler
	{
		public Material? Material { get; set; }

		public Damage Damage { get; set; }
	}
}
