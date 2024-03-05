using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Schedulers;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : Material
	{
		Scale? Strength { get; }

		Collided Body { get; }

		int Speed { get; set; }

		ICollection<SchedulerRunner> Schedulers { get; }

		void OnTick();

		void SetCourse(Course? course);

		void Collision(Collided thisCollided, Material otherMaterial, Collided otherCollided);
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy<TTarget>
		where TTarget : Material
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public Scale? Strength { get => Target.Strength; protected set => SetValue(x => x.Strength, value); }

		public Collided Body { get => Target.Body; }

		public ICollection<SchedulerRunner> Schedulers => CreateCollectionProxy(Target.Schedulers);

		public virtual void OnTick()
		{
			foreach (var scheduler in Schedulers.Select(Owner.CreateProxy).Cast<ISchedulerRunnerProxy>())
			{
				scheduler.OnTick();
			}
		}

		public virtual void Collision(Collided thisCollided, Material otherMaterial, Collided otherCollided)
		{
			if (Target != otherMaterial && otherCollided.IsRigid)
			{
				((ICollidedProxy)Owner.CreateProxy(Body)).Break();
			}
		}

		public void SetCourse(Course? course)
		{
			if (Target.GetControlVector() is { } controlVector)
			{
				var offset = course?.ToOffset() ?? Point.Empty;
				var vector = new Vector2(offset.X, offset.Y) * Speed;

				var controlVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(controlVector);
				controlVectorProxy.Value = vector;
			}
		}

		public int Speed { get => Target.Speed; set => SetValue(x => x.Speed, value); }
	}
}
