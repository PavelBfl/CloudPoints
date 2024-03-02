using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlayerCharacterProxy : IMaterialProxy<PlayerCharacter>
	{
		new Scale Strength { get; }

		void CreateProjectile(Course course);
	}

	internal sealed class PlayerCharacterProxy : MaterialProxy<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
		}

		public new Scale Strength => base.Strength ?? throw new InvalidOperationException();

		public IScaleProxy Cooldown => (IScaleProxy)Owner.CreateProxy(Target.Cooldown);

		public override void OnTick()
		{
			base.OnTick();

			if (Strength.Value == 0)
			{
				Owner.GetPlaygroundProxy().PlayerCharacter = null;
			}
			else
			{
				Cooldown.Decrement();
			}
		}

		public override void Collision(Collided thisCollided, Material otherMaterial, Collided otherCollided)
		{
			if (otherMaterial is Item item)
			{
				var itemsProxy = CreateListProxy(Owner.Playground.Items);
				itemsProxy.Remove(item);
				var itemBody = (ICollidedProxy?)Owner.CreateProxy(item.Body);
				itemBody?.Clear();
				CreateListProxy(Target.Items).Add(item);

				Speed -= item.Speed;
				Cooldown.SetMin();
				Cooldown.Max -= item.AttackCooldown;

				Strength.Value += item.AddStrength;
			}
			else if ((otherMaterial as Projectile)?.Creator != Target)
			{
				base.Collision(thisCollided, otherMaterial, otherCollided);
			}
		}

		public void CreateProjectile(Course course)
		{
			const int SIZE = 10;

			if (Cooldown.Value == 0)
			{
				var border = Body.Current.Bounds;
				var center = new Point(
					border.X + border.Width / 2,
					border.Y + border.Height / 2
				);

				var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile()
				{
					Creator = Target,
					Body = new Collided(),
					Damage = AggregateDamage(10),
					Speed = 5,
				});

				projectile.Body.Current = new ShapeCell(
					Owner.Playground.IntersectionContext,
					new Rectangle(
						center.X - SIZE / 2,
						center.Y - SIZE / 2,
						SIZE,
						SIZE
					)
				);

				var courseVector = course.ToOffset();
				var scheduler = new SchedulerVector()
				{
					Collided = projectile.Body,
					Vectors = { new Vector2(courseVector.X, courseVector.Y) },
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
					Begin = Owner.TimeAxis.Count,
					Scheduler = schedulerUnion,
				});

				var projectilesProxy = CreateListProxy(Owner.Playground.Projectiles);
				projectilesProxy.Add(projectile.Target);
				Cooldown.SetMax();
			}
		}

		private Damage AggregateDamage(int value = 0, DamageKind kind = DamageKind.None)
		{
			foreach (var settings in Target.Items.Select(x => x.DamageSetting))
			{
				if (settings is { })
				{
					value += settings.Value;
					kind |= settings.Kind;
				}
			}

			return new Damage()
			{
				Value = value,
				Kind = kind,
			};
		}
	}
}
