﻿using System;
using System.Drawing;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Intersection;
using StepFlow.Master.Proxies.Schedulers;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IEnemyProxy : IMaterialProxy<Enemy>
	{
		Collided Vision { get; }
	}

	internal sealed class EnemyProxy : MaterialProxy<Enemy>, IEnemyProxy
	{
		public EnemyProxy(PlayMaster owner, Enemy target) : base(owner, target)
		{
		}

		public Collided Vision => Target.GetVisionRequired();

		public Scale Cooldown => Target.GetCooldownRequired();

		public override void OnTick()
		{
			base.OnTick();

			if (Strength?.Value == 0)
			{
				var enemiesProxy = Owner.CreateListProxy(Owner.Playground.Enemies);
				enemiesProxy.Remove(Target);

				var itemPosition = Body.Current.Bounds.GetCenter();
				Owner.CreateItem.Execute(new Scripts.CreateItem.Parameters()
				{
					X = itemPosition.X,
					Y = itemPosition.Y,
					Kind = Target.ReleaseItem
				});

				var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
				bodyProxy.Clear();
				
				var visionProxy = (ICollidedProxy)Owner.CreateProxy(Vision);
				visionProxy.Clear();
			}
			else
			{
				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.Decrement();

				var center = Body.Current.Bounds.GetCenter();
				var visionPlace = RectangleExtensions.Create(center, 50);
				var visionProxy = (IShapeContainerProxy)Owner.CreateProxy(Vision.Current);
				visionProxy.Reset(visionPlace);
			}
		}

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			base.Collision(thisCollided, otherMaterial, otherCollided);

			if (Target != otherMaterial)
			{
				if (thisCollided.Collided == Vision && otherMaterial == Owner.Playground.PlayerCharacter)
				{
					CreateProjectile(otherMaterial);
				}
				else if (otherCollided.Collided.IsRigid &&
					thisCollided.Collided == Body &&
					Target.TryGetControlVector(out var runner, out _, out var controlVector) &&
					runner.Current is { })
				{
					var runnerProxy = (ISchedulerRunnerProxy)Owner.CreateProxy(runner);
					runnerProxy.Current = null;

					var rotate = Matrix3x2.CreateRotation(MathF.PI / 2);
					var controlVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(controlVector);
					controlVectorProxy.Value = Vector2.Transform(controlVector.Value, rotate);
				}
			}
		}

		private void CreateProjectile(Material other)
		{
			if (Cooldown.Value == 0)
			{
				var border = Body.Current.Bounds;
				var center = new Point(
					border.X + border.Width / 2,
					border.Y + border.Height / 2
				);

				var otherBorder = other.GetBodyRequired().Current.Bounds;
				var otherCenter = new Point(
					otherBorder.X + otherBorder.Width / 2,
					otherBorder.Y + otherBorder.Height / 2
				);

				const int SIZE = 10;
				var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile()
				{
					Creator = Target,
					Body = new Collided(),
					Damage = new Damage()
					{
						Value = 10,
					},
					Speed = 5,
				});

				projectile.Body.Current.Add(new Rectangle(
					center.X - SIZE / 2,
					center.Y - SIZE / 2,
					SIZE,
					SIZE
				));

				var courseVector = Vector2.Normalize(new Vector2(otherCenter.X - center.X, otherCenter.Y - center.Y)) * 5;

				var scheduler = new SchedulerVector()
				{
					Collided = projectile.Body,
					Vectors = { new CourseVector() { Value = courseVector } },
				};
				var schedulerLimit = new SchedulerLimit()
				{
					Source = scheduler,
					Range = new Scale()
					{
						Max = 12000,
					},
				};
				var schedulerCompletion = new SchedulerCollection()
				{
					Turns =
					{
						new Turn(
							0,
							new RemoveProjectile()
							{
								Projectile = projectile.Target,
							}
						)
					},
				};
				var schedulerUnion = new SchedulerUnion()
				{
					Schedulers = { schedulerLimit, schedulerCompletion },
				};

				projectile.Schedulers.Add(new SchedulerRunner()
				{
					Scheduler = schedulerUnion,
				});

				var projectilesProxy = Owner.CreateListProxy(Owner.Playground.Projectiles);
				projectilesProxy.Add(projectile.Target);
				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.SetMax();
			}
		}
	}
}
