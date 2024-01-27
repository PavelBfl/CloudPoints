using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface ISchedulerProxy<out TEffect> : IElementBaseProxy<TEffect>
		where TEffect : Scheduler
	{
		new Subject? Target { get; set; }

		Scale? Cooldown { get; set; }

		int? RepeatCount { get; set; }

		void OnTick();
	}

	internal abstract class SchedulerProxy<TEffect> : ElementBaseProxy<TEffect>, ISchedulerProxy<TEffect>
		where TEffect : Scheduler
	{
		public SchedulerProxy(PlayMaster owner, TEffect target) : base(owner, target)
		{
		}

		public new Subject? Target { get => base.Target.Target; set => SetValue(x => x.Target, value); }

		public Scale? Cooldown { get => base.Target.Cooldown; set => SetValue(x => x.Cooldown, value); }

		public int? RepeatCount { get => base.Target.RepeatCount; set => SetValue(x => x.RepeatCount, value); }

		public virtual void OnTick()
		{
			if (RepeatCount == 0)
			{
				return;
			}

			var cooldownProxy = (IScaleProxy?)Owner.CreateProxy(Cooldown);
			if (cooldownProxy is null || cooldownProxy.Value == cooldownProxy.Max)
			{
				InnerOnTick();

				if (cooldownProxy is { })
				{
					cooldownProxy.Value = 0;
				}

				RepeatCount--;
			}
			else
			{
				cooldownProxy.Increment();
			}
		}

		protected abstract void InnerOnTick();
	}

	public interface IPathSchedulerProxy : ISchedulerProxy<PathScheduler>
	{
		int CurrentPathIndex { get; set; }
		IList<Course> Path { get; }
	}

	internal sealed class PathSchedulerProxy : SchedulerProxy<PathScheduler>, IPathSchedulerProxy
	{
		public PathSchedulerProxy(PlayMaster owner, PathScheduler target) : base(owner, target)
		{
		}

		public int CurrentPathIndex { get => ((IReadOnlyProxyBase<PathScheduler>)this).Target.CurrentPathIndex; set => SetValue(x => x.CurrentPathIndex, value); }

		public IList<Course> Path { get => CreateListProxy(((IReadOnlyProxyBase<PathScheduler>)this).Target.Path); }

		protected override void InnerOnTick()
		{
			// TODO переделать на универсальный объект
			if (Target is Projectile { CurrentAction: null } projectile)
			{
				var projectileProxy = (IProjectileProxy)Owner.CreateProxy(projectile);
				projectileProxy.SetCourse(Path[CurrentPathIndex]);

				var newIndex = CurrentPathIndex + 1;
				CurrentPathIndex = newIndex < Path.Count ? newIndex : 0;
			}
		}
	}
}
