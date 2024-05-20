using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Core.Tracks;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;

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
			else if ((otherMaterial as Projectile)?.Immunity.Contains(Target) != true)
			{
				base.Collision(thisCollided, otherMaterial, otherCollided);
			}
		}

		public void CreateProjectile(float radians, PlayerAction action)
		{
			return;
			const int SIZE = 10;

			if (action != PlayerAction.None && Cooldown.Value == 0)
			{
				var center = Body.Current.Bounds.GetCenter();
				var matrixRotation = Matrix3x2.CreateRotation(radians);
				var courseVector = Vector2.Transform(new Vector2(0.05f, 0), matrixRotation);

				switch (action)
				{
					case PlayerAction.None:
						break;
					case PlayerAction.Main:
						Owner.CreateProjectile(
							center,
							SIZE,
							courseVector,
							AggregateDamage(value: 10),
							TimeTick.FromSeconds(1),
							Target
						);
						break;
					case PlayerAction.Auxiliary:
						// TODO Дуга
						var arcCourse = courseVector * 10;
						var arcDuration = TimeTick.FromSeconds(0.1f);
						var arcDistance = arcDuration.Ticks / (SchedulerVector.MAX_DURATION / arcCourse.Length());
						var arcOffset = Vector2.Transform(courseVector * arcDistance, Matrix3x2.CreateRotation(MathF.PI / -4));
						CreateArc(
							center + new Size((int)arcOffset.X, (int)arcOffset.Y),
							SIZE,
							Vector2.Transform(arcCourse, Matrix3x2.CreateRotation(MathF.PI / 4)),
							AggregateDamage(value: 10),
							arcDuration,
							Target
						);

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
						//Owner.CreateProjectile(
						//	center,
						//	SIZE,
						//	courseVector * 10,
						//	new Damage() { Push = courseVector * 10 },
						//	TimeTick.FromFrames(5),
						//	Target
						//);
						break;
					default: throw EnumNotSupportedException.Create(action);
				}

				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.SetMax();
			}
		}

		private void CreateArc(Point center, int radius, Vector2 course, Damage damage, int duration, Subject? creator)
		{
			var bodyCurrent = RectangleExtensions.Create(center, radius);
			var projectile = new Projectile()
			{
				Name = "Projectile",
				Body = new Collided()
				{
					Current = { bodyCurrent },
					Position = new Vector2(bodyCurrent.X, bodyCurrent.Y),
				},
				Damage = damage,
				Reusable = true,
				Speed = 100,
				Track = new TrackBuilder()
				{
					Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(0.01f)),
					Change = new TrackChange()
					{
						Thickness = 2,
						Size = new Vector2(-0.005f),
						View = TrackView.None,
					},
				},
			};

			if (creator is { })
			{
				projectile.Immunity.Add(creator);
			}

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
