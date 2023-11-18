using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : Material
	{
		IScaleProxy? Strength { get; }

		void OnTick();

		void SetCourse(Course course);
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy<TTarget>
		where TTarget : Material
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public IScaleProxy? Strength { get => (IScaleProxy?)Owner.CreateProxy(Target.Strength); protected set => SetValue(x => x.Strength, value?.Target); }

		public ICollidedProxy Body { get => (ICollidedProxy)Owner.CreateProxy(Target.Body); }

		public IScheduledProxy Scheduler { get => (IScheduledProxy)Owner.CreateProxy(Target.Scheduler); }

		public virtual void OnTick()
		{
		}

		public virtual void Collision(ICollidedProxy other)
		{
			((ICollidedProxy)this).Break();
		}

		public void SetCourse(Course course)
		{
			var factor = course.GetFactor();

			var setCourse = (ISetCourseProxy)Owner.CreateProxy(new SetCourse()
			{
				Collided = Body.Target,
				Course = course,
			});


			Scheduler.Enqueue(factor, setCourse);
		}
	}
}
