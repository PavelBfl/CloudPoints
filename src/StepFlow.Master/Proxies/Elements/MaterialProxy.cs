using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : Material
	{
		Scale? Strength { get; }

		Collided Body { get; }

		int Speed { get; set; }

		Action? CurrentAction { get; set; }

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

		public Scale? Strength { get => Target.Strength; protected set => SetValue(x => x.Strength, value); }

		public Collided Body { get => Target.Body; }

		public Action? CurrentAction { get => Target.CurrentAction; set => SetValue(x => x.CurrentAction, value); }

		public virtual void OnTick()
		{
			if (CurrentAction is { } currentAction)
			{
				if (Owner.TimeAxis.Current == (currentAction.Begin + currentAction.Duration))
				{
					var executorProxy = (ITurnExecutor?)Owner.CreateProxy(currentAction.Executor);
					executorProxy?.Execute();
					CurrentAction = null;
				}
			}
		}

		public virtual void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (Target != otherMaterial.Target && otherCollided.IsRigid)
			{
				((ICollidedProxy)Owner.CreateProxy(Body)).Break();
			}
		}

		public void SetCourse(Course course)
		{
			var factor = course.GetFactor();

			CurrentAction = new Action()
			{
				Begin = Owner.TimeAxis.Current,
				Duration = factor * Speed,
				Executor = new SetCourse()
				{
					Collided = Body,
					Course = course,
				},
			};
		}

		public int Speed { get => Target.Speed; set => SetValue(x => x.Speed, value); }
	}
}
