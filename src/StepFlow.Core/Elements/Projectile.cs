using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Projectile : Material
	{
		public Subject? Creator { get; set; }

		private Damage? damage;

		public Damage? Damage { get => damage; set => SetComponent(ref damage, value); }

		public ICollection<SchedulerRunner> Schedulers { get; } = new HashSet<SchedulerRunner>();
	}
}
