using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerDamageProxy : ISchedulerProxy<SchedulerDamage>
	{
	
	}

	internal class SchedulerDamageProxy : SchedulerProxy<SchedulerDamage>, ISchedulerDamageProxy
	{
		public SchedulerDamageProxy(PlayMaster owner, SchedulerDamage target) : base(owner, target)
		{
		}

		public Material? Material { get => Target.Material; set => SetValue(x => x.Material, value); }

		public Damage Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }

		public override void Next()
		{
			Current = new Turn(
				TimeTick.FromSeconds(1),
				new ChangeStrength()
				{
					Damage = Damage,
					Material = Material,
				}
			);
		}
	}
}
