using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : Material
	{
		IScaleProxy? Strength { get; }

		ICollidedProxy Body { get; }

		IScheduledProxy Scheduler { get; }

		void OnTick();

		void SetCourse(Course course);

		void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided);
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

		public virtual void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (otherCollided.IsRigid)
			{
				Body.Break(); 
			}
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
