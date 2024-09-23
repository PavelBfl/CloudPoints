using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Domains.Tracks;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.States;

namespace StepFlow.Master.Proxies.Elements
{
	public enum PlayerAction
	{
		Main,
		Auxiliary,
	}

	public interface IPlayerCharacterProxy : IMaterialProxy<PlayerCharacter>
	{
		CharacterSkill MainSkill { get; set; }

		CharacterSkill AuxiliarySkill { get; set; }
		Scale Cooldown { get; set; }
		IList<Item> Items { get; }

		void CreateProjectile(float radians, PlayerAction action);

		void CopyFrom(PlayerCharacterDto original)
		{
			NullValidate.ThrowIfArgumentNull(original, nameof(original));

			((IMaterialProxy<PlayerCharacter>)this).CopyFrom(original);
			Cooldown = original.Cooldown;
			MainSkill = original.MainSkill;
			AuxiliarySkill = original.AuxiliarySkill;
			var itemsProxy = Owner.CreateListProxy(Items);
			itemsProxy.Clear();
			foreach (var item in original.Items)
			{
				itemsProxy.Add(new Item(Owner.Playground.Context, item));
			}
		}

		bool CanJump();
	}

	internal sealed class PlayerCharacterProxy : MaterialProxy<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
		}

		public Scale Cooldown { get => Target.Cooldown; set => SetValue(value); }

		public CharacterSkill MainSkill { get => Target.MainSkill; set => SetValue(value); }

		public CharacterSkill AuxiliarySkill { get => Target.AuxiliarySkill; set => SetValue(value); }

		public IList<Item> Items => Target.Items;

		public override void OnTick()
		{
			base.OnTick();

			if (Strength.Value == 0)
			{
				Owner.GetPlaygroundItemsProxy().Remove(Target);
			}
			else
			{
				Cooldown--;
			}
		}

		protected override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (otherMaterial is Item item)
			{
				Owner.GetPlaygroundItemsProxy().Remove(item);
				var itemBody = (ICollidedProxy?)Owner.CreateProxy(item.Body);
				itemBody?.Clear();
				Owner.CreateListProxy(Target.Items).Add(item);

				Speed += item.Speed;
				Cooldown = Scale.CreateByMin(Cooldown.Max - item.AttackCooldown);
				Strength += item.AddStrength;
			}
			else if ((otherMaterial as Projectile)?.Immunity.Contains(Target) != true)
			{
				base.Collision(thisCollided, otherMaterial, otherCollided);
			}
		}

		public void CreateProjectile(float radians, PlayerAction action)
		{
			const int SIZE = 10;

			if (Cooldown.Value == 0)
			{
				var skill = action switch
				{
					PlayerAction.Main => MainSkill,
					PlayerAction.Auxiliary => AuxiliarySkill,
					_ => throw EnumNotSupportedException.Create(action),
				};

				var center = Body.GetCurrentRequired().Bounds.GetCenter();
				var matrixRotation = Matrix3x2.CreateRotation(radians);
				var courseVector = Vector2.Transform(new Vector2(0.05f, 0), matrixRotation);

				switch (skill)
				{
					case CharacterSkill.Projectile:
						Owner.CreateProjectile(
							center,
							SIZE,
							courseVector,
							AggregateDamage(value: 10),
							TimeTick.FromSeconds(1),
							Target,
							ReusableKind.None
						);
						break;
					case CharacterSkill.Arc:
						var arcDuration = TimeTick.FromSeconds(0.2f);
						var arcRadius = 40;
						var arcSpeed = 0.05f;
						var arcRouteDistance = arcDuration.Ticks * arcSpeed;

						var m = Matrix3x2.CreateTranslation(0, -arcRadius) *
							Matrix3x2.CreateRotation(MathF.PI / 2) *
							Matrix3x2.CreateRotation(-(arcRouteDistance / arcRadius / 2)) *
							Matrix3x2.CreateRotation(radians) *
							Matrix3x2.CreateTranslation(center.X, center.Y);

						var arcPosition = Vector2.Transform(Vector2.Zero, m);
						var arcCourse = Vector2.Transform(new Vector2(arcSpeed, 0), m);
						CreateArc(
							new Point((int)arcPosition.X, (int)arcPosition.Y),
							SIZE,
							arcCourse - arcPosition,
							AggregateDamage(value: 10),
							arcDuration,
							Target
						);
						break;
					case CharacterSkill.Push:
						Owner.CreateProjectile(
							center,
							SIZE,
							courseVector,
							new Damage() { Push = courseVector * 0.01f },
							TimeTick.FromSeconds(0.1f),
							Target,
							ReusableKind.NotSave
						);
						break;
					case CharacterSkill.Dash:
						if (Target.States.SingleOrDefault(x => x.Kind == StateKind.Dash) is { } dashState)
						{
							var dashStateProxy = (IStateProxy)Owner.CreateProxy(dashState);
							dashStateProxy.Vector = courseVector;
						}
						else
						{
							var statesProxy = Owner.CreateCollectionProxy(Target.States);
							statesProxy.Add(new State(Owner.Playground.Context)
							{
								Kind = StateKind.Dash,
								TotalCooldown = TimeTick.FromSeconds(0.1f),
								Arg0 = courseVector.X * 5,
								Arg1 = courseVector.Y * 5,
							});
						}
						break;
					default: throw EnumNotSupportedException.Create(skill);
				}

				Cooldown = Cooldown.SetMax();
			}
		}

		private void CreateArc(Point center, int radius, Vector2 course, Damage damage, int duration, Subject? creator)
		{
			var projectile = new Projectile(Owner.Playground.Context)
			{
				Name = "Projectile",
				Body =
				{
					Current = Owner.CreateShape(RectangleExtensions.Create(center, radius)),
				},
				Damage = damage,
				Reusable = ReusableKind.Save,
				Speed = 100,
				Course = course,
				States =
				{
					new State(Owner.Playground.Context)
					{
						Kind = StateKind.Remove,
						TotalCooldown = duration,
					},
					new State(Owner.Playground.Context)
					{
						Kind = StateKind.Arc,
					},
				},
				Track = new TrackBuilder(Owner.Playground.Context)
				{
					Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(0.01f)),
					Change = new TrackChange(Owner.Playground.Context)
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

		public bool CanJump() => RigidExists(new Point(0, 1));
	}
}
