using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Schedulers;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>, IPlaygroundUsed
		where TTarget : Material
	{
		Scale? Strength { get; }

		Collided Body { get; }

		int Speed { get; set; }

		int Weight { get; set; }

		ICollection<SchedulerRunner> Schedulers { get; }

		void OnTick();

		void SetCourse(float? radian);


		void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided);

		void ChangeStrength(Damage damage);
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy<TTarget>
		where TTarget : Material
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public Scale? Strength { get => Target.Strength; protected set => SetValue(value); }

		public virtual void ChangeStrength(Damage damage)
		{
			if ((IScaleProxy?)Owner.CreateProxy(Strength) is { } strengthProxy)
			{
				strengthProxy.Add(-damage.Value);
			}
		}

		public Collided Body { get => Target.GetBodyRequired(); }

		public ICollection<SchedulerRunner> Schedulers => Target.Schedulers;

		public virtual void OnTick()
		{
			var removedSchedulers = new List<SchedulerRunner>();

			foreach (var scheduler in Schedulers.Select(Owner.CreateProxy).Cast<ISchedulerRunnerProxy>())
			{
				if (scheduler.OnTick())
				{
					removedSchedulers.Add(scheduler.Target);
				}
			}

			if (removedSchedulers.Count > 0)
			{
				var schedulersProxy = Owner.CreateCollectionProxy(Schedulers);
				foreach (var scheduler in removedSchedulers)
				{
					schedulersProxy.Remove(scheduler);
				}
			}
		}

		public virtual void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (Target != otherMaterial && otherCollided.Collided.IsRigid && thisCollided.Collided.IsRigid)
			{
				((ICollidedProxy)Owner.CreateProxy(Body)).Break();
			}
		}

		public void SetCourse(float? radians)
		{
			if (Target.GetCourseVector(Material.SHEDULER_CONTROL_NAME) is { } path)
			{
				var courseVector = Vector2.Zero;
				if (radians is { })
				{
					var matrixRotation = Matrix3x2.CreateRotation(radians.Value);
					courseVector = Vector2.Transform(new Vector2(0.1f, 0), matrixRotation);
				}

				var controlVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(path.CourseVector);
				controlVectorProxy.Value = courseVector * Speed;
			}
		}

		public virtual void Begin()
		{
			var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
			bodyProxy.Register();
		}

		public virtual void End()
		{
			var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
			bodyProxy.Unregister();
		}

		public int Speed { get => Target.Speed; set => SetValue(value); }

		public int Weight
		{
			get => Target.Weight;
			set
			{
				if (value < 0 || Material.MAX_WEIGHT < value)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				SetValue(value);
			}
		}
	}
}
