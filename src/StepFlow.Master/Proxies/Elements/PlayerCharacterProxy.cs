using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlayerCharacterProxy : IMaterialProxy<PlayerCharacter>
	{
		new IScaleProxy Strength { get; }

		void CreateProjectile(Course course);
	}

	internal sealed class PlayerCharacterProxy : MaterialProxy<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
		}

		public new IScaleProxy Strength => base.Strength ?? throw new InvalidOperationException();

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
				Items.Add(itemProxy);

				Speed -= itemProxy.Speed;
			}
			else if ((otherMaterial as IProjectileProxy)?.Creator?.Target != Target)
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
					Body = new Collided()
					{
						Current = Owner.GetPlaygroundProxy().CreateCell(new Rectangle(
							center.X - SIZE / 2,
							center.Y - SIZE / 2,
							SIZE,
							SIZE
						)).Target,
					},
					Damage = AggregateDamage(10),
					Speed = 5,
					CurrentPathIndex = 0,
				});

				for (var i = 0; i < 100; i++)
				{
					projectile.Path.Add(course);
				}

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
