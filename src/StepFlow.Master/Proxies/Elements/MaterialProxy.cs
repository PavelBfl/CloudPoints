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

		int Speed { get; set; }

		IActionProxy? CurrentAction { get; set; }

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

		public IActionProxy? CurrentAction { get => (IActionProxy?)Owner.CreateProxy(Target.CurrentAction); set => SetValue(x => x.CurrentAction, value?.Target); }

		public virtual void OnTick()
		{
			if (CurrentAction is { } currentAction)
			{
				if (Owner.Time == (currentAction.Begin + currentAction.Duration))
				{
					currentAction.Executor?.Execute();
					CurrentAction = null;
				}
			}
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

			CurrentAction = (IActionProxy)Owner.CreateProxy(new Action()
			{
				Begin = Owner.Time,
				Duration = factor * Speed,
				Executor = new SetCourse()
				{
					Collided = Body.Target,
					Course = course,
				},
			});
		}

		public int Speed { get => Target.Speed; set => SetValue(x => x.Speed, value); }
	}
}
