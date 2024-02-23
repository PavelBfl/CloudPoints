﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
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

		public IList<IItemProxy> Items => new ListItemsProxy<Item, IList<Item>, IItemProxy>(Owner, Target.Items);

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

		public override void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (otherMaterial is IItemProxy itemProxy)
			{
				Owner.GetPlaygroundProxy().Items.Remove(itemProxy);
				var itemBody = (ICollidedProxy)Owner.CreateProxy(itemProxy.Body);
				itemBody.Current = null;
				itemBody.Next = null;
				Items.Add(itemProxy);

				Speed -= itemProxy.Speed;
				Cooldown.SetMin();
				Cooldown.Max -= itemProxy.AttackCooldown;

				Strength.Value += itemProxy.AddStrength;
			}
			else if ((otherMaterial as IProjectileProxy)?.Creator != Target)
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
				courseVector.X *= 300;
				courseVector.Y *= 300;
				var point = projectile.Body.Current.Bounds.GetCenter();
				point.X += courseVector.X;
				point.Y += courseVector.Y;

				var scheduler = new SchedulerVector()
				{
					Collided = projectile.Body,
					Vectors = { new Vector2(point.X, point.Y) },
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

				Owner.GetPlaygroundProxy().Projectiles.Add(projectile);
				Cooldown.SetMax();
			}
		}

		private Damage AggregateDamage(int value = 0, DamageKind kind = DamageKind.None)
		{
			foreach (var settings in Items.Select(x => x.DamageSettings))
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
