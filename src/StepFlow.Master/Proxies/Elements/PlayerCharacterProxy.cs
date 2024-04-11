using System;
using System.Linq;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Intersection;
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

		public void CreateProjectile(Course course)
		{
			const int SIZE = 10;

			if (Cooldown.Value == 0)
			{
				var courseVector = course.ToOffset();
				Owner.CreateProjectile(
					Body.Current.Bounds.GetCenter(),
					SIZE,
					new Vector2(courseVector.X, courseVector.Y) * 80,
					AggregateDamage(10),
					TimeTick.FromSeconds(4),
					Target
				);

				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.SetMax();
			}
		}

		private Damage AggregateDamage(int value = 0, DamageKind kind = DamageKind.None)
		{
			foreach (var settings in Target.Items.Select(x => x.DamageSetting))
			{
				value += settings.Value;
				kind |= settings.Kind;
			}

			return new Damage()
			{
				Value = value,
				Kind = kind,
			};
		}
	}
}
