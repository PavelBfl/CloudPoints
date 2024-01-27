using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		Subject? Creator { get; set; }

		Damage? Damage { get; set; }

		int CurrentPathIndex { get; set; }

		IList<Course> Path { get; }

		ICollection<Scheduler> Schedulers { get; }

		void AddScheduler(Scheduler scheduler);
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public Subject? Creator { get => Target.Creator; set => SetValue(x => x.Creator, value); }

		public Damage? Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }

		public int CurrentPathIndex { get => Target.CurrentPathIndex; set => SetValue(x => x.CurrentPathIndex, value); }

		public IList<Course> Path => CreateListProxy(Target.Path);

		public ICollection<Scheduler> Schedulers => CreateCollectionProxy(Target.Schedulers);

		public void AddScheduler(Scheduler scheduler)
		{
			var schedulerProxy = (ISchedulerProxy)Owner.CreateProxy(scheduler);
			schedulerProxy.Target = Target;
			Schedulers.Add(scheduler);
		}

		public override void OnTick()
		{
			base.OnTick();

			foreach (var scheduler in Schedulers)
			{
				var schedulerProxy = (ISchedulerProxy)Owner.CreateProxy(scheduler);
				schedulerProxy.OnTick();
			}

			// TODO временно
			return;

			if (CurrentPathIndex >= Path.Count)
			{
				Owner.GetPlaygroundProxy().Projectiles.Remove(this);
				Body.Current = null;
				Body.Next = null;
			}
			else if (CurrentAction is null)
			{
				SetCourse(Path[CurrentPathIndex]);
				CurrentPathIndex++;
			}
			
		}

		public override void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (Creator != otherMaterial.Target && otherCollided.IsRigid)
			{
				if (otherMaterial.Strength is { } strength)
				{
					var strengthProxy = (IScaleProxy)Owner.CreateProxy(strength);
					strengthProxy.Add(-Damage.Value);
				}

				Owner.GetPlaygroundProxy().Projectiles.Remove(this);
				Body.Current = null;
				Body.Next = null;
			}
		}
	}
}
