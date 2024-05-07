﻿using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;
using static System.Net.Mime.MediaTypeNames;

namespace StepFlow.Master.Proxies.Elements
{
	public enum PlayerAction
	{
		None,
		Main,
		Auxiliary,
	}

	public interface IPlayerCharacterProxy : IMaterialProxy<PlayerCharacter>
	{
		new Scale Strength { get; }

		void CreateProjectile(float radians, PlayerAction action);
	}

	internal sealed class PlayerCharacterProxy : MaterialProxy<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
		}

		public new Scale Strength => base.Strength ?? throw new InvalidOperationException();

		public Scale Cooldown => Target.GetCooldownRequired();

		public override void OnTick()
		{
			base.OnTick();

			if (Strength.Value == 0)
			{
				Owner.GetPlaygroundItemsProxy().Remove(Target);
			}
			else
			{
				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.Decrement();
			}
		}

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (otherMaterial is Item item)
			{
				Owner.GetPlaygroundItemsProxy().Remove(item);
				var itemBody = (ICollidedProxy?)Owner.CreateProxy(item.Body);
				itemBody?.Clear();
				Owner.CreateListProxy(Target.Items).Add(item);

				Speed += item.Speed;
				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.SetMin();
				cooldownProxy.Max -= item.AttackCooldown;

				Strength.Value += item.AddStrength;
			}
			else if ((otherMaterial as Projectile)?.Creator != Target)
			{
				base.Collision(thisCollided, otherMaterial, otherCollided);
			}
		}

		public void CreateProjectile(float radians, PlayerAction action)
		{
			const int SIZE = 10;

			if (action != PlayerAction.None && Cooldown.Value == 0)
			{
				var center = Body.Current.Bounds.GetCenter();
				var matrixRotation = Matrix3x2.CreateRotation(radians);
				var courseVector = Vector2.Transform(new Vector2(1, 0), matrixRotation);

				switch (action)
				{
					case PlayerAction.None:
						break;
					case PlayerAction.Main:
						Owner.CreateProjectile(
							center,
							SIZE,
							courseVector * 5,
							AggregateDamage(value: 10),
							TimeTick.FromSeconds(4),
							Target
						);
						break;
					case PlayerAction.Auxiliary:
						// TODO Дуга
						//CreateArc(
						//	center,
						//	SIZE,
						//	courseVector * 10,
						//	AggregateDamage(value: 10),
						//	TimeTick.FromSeconds(0.1f),
						//	Target
						//);

						// TODO Рывок
						//var schedulersProxy = Owner.CreateCollectionProxy(Schedulers);
						//schedulersProxy.Add(new SchedulerRunner()
						//{
						//	Scheduler = new SchedulerLimit()
						//	{
						//		Range = Scale.Create(TimeTick.FromSeconds(0.1f)),
						//		Source = new SchedulerVector()
						//		{
						//			Collided = Target.Body,
						//			Vectors =
						//			{
						//				new CourseVector()
						//				{
						//					Value = courseVector * 20,
						//				},
						//			},
						//		},
						//	},
						//});

						// TODO Толчок
						Owner.CreateProjectile(
							center,
							SIZE,
							courseVector * 10,
							new Damage() { Push = courseVector * 10 },
							TimeTick.FromFrames(5),
							Target
						);
						break;
					default: throw EnumNotSupportedException.Create(action);
				}

				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.SetMax();
			}
		}

		private void CreateArc(Point center, int radius, Vector2 course, Damage damage, int duration, Subject? creator)
		{
			var projectile = new Projectile()
			{
				Name = "Projectile",
				Creator = creator,
				Body = new Collided()
				{
					Current = { RectangleExtensions.Create(center, radius) },
				},
				Damage = damage,
				Speed = 100,
			};

			var schedulerUnion = new SchedulerUnion()
			{
				Schedulers =
				{
					new SchedulerLimit()
					{
						Source = new SchedulerVector()
						{
							Collided = projectile.Body,
							Vectors =
							{
								new CourseVector()
								{
									Value = course,
									Delta = Matrix3x2.CreateRotation((MathF.PI / 2) / duration),
								},
							},
						},
						Range = Scale.Create(duration),
					},
					new SchedulerCollection()
					{
						Turns =
						{
							new Turn(
								0,
								new RemoveItem()
								{
									Item = projectile,
								}
							)
						},
					},
				},
			};

			projectile.Schedulers.Add(new SchedulerRunner()
			{
				Scheduler = schedulerUnion,
			});

			Owner.GetPlaygroundItemsProxy().Add(projectile);
		}

		private Damage AggregateDamage(int value = 0, DamageKind kind = DamageKind.None, Vector2 push = default)
		{
			foreach (var settings in Target.Items.Select(x => x.DamageSetting))
			{
				value += settings.Value;
				kind |= settings.Kind;
			}

			return new Damage()
			{
				Value = value,
				Push = push,
				Kind = kind,
			};
		}
	}
}
