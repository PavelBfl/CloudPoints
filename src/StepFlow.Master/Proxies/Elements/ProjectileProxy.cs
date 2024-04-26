﻿using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Schedulers;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		Subject? Creator { get; set; }

		Damage Damage { get; set; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public Subject? Creator { get => Target.Creator; set => SetValue(value); }

		public Damage Damage { get => Target.Damage; set => SetValue(value); }

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (Creator != otherMaterial && otherCollided.Collided.IsRigid)
			{
				if (otherMaterial.Strength is { } strength)
				{
					var strengthProxy = (IScaleProxy)Owner.CreateProxy(strength);
					strengthProxy.Add(-Damage.Value);
				}

				if (Damage.Push != Vector2.Zero)
				{
					var controlVector = GetOrCreateControlVector(otherMaterial, Material.SHEDULER_INERTIA_NAME);
					var courseVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(controlVector.CourseVector);
					courseVectorProxy.Value = Damage.Push;
				}

				Owner.GetPlaygroundItemsProxy().Remove(Target);
			}
		}

		private Material.CourseVectorPath GetOrCreateControlVector(Material material, string vectorName)
		{
			var result = material.GetCourseVector(vectorName);
			if (result is null)
			{
				var schedulersProxy = Owner.CreateCollectionProxy(material.Schedulers);
				var courseVector = new CourseVector()
				{
					Name = vectorName,
				};

				var schedulerVector = new SchedulerVector()
				{
					Collided = material.Body,
					Vectors = { courseVector },
				};

				var schedulerRunner = new SchedulerRunner()
				{
					Scheduler = schedulerVector,
				};

				result = new Material.CourseVectorPath(schedulerRunner, schedulerVector, courseVector);
				schedulersProxy.Add(schedulerRunner);
			}

			return result;
		}
	}
}
