using System;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlayerCharacterProxy : IProxyBase<PlayerCharacter>, IMaterialProxy
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

		public IScaleProxy Cooldown { get => (IScaleProxy)Owner.CreateProxy(Target.Cooldown); }

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

		public void CreateProjectile(Course course)
		{
			const int SIZE = 10;

			if (Cooldown.Value == 0)
			{
				var border = Current.Border;
				var center = new Point(
					border.X + border.Width / 2,
					border.Y + border.Height / 2
				);

				var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile(Target.Context)
				{
					Creator = Target,
					Current = new Cell()
					{
						Border = new Rectangle(
							center.X - SIZE / 2,
							center.Y - SIZE / 2,
							SIZE,
							SIZE
						),
					},
					Damage = new Damage(Target.Context)
					{
						Value = 10,
					},
				});

				for (var i = 0; i < 100; i++)
				{
					projectile.SetCourse(course);
				}

				Owner.GetPlaygroundProxy().Projectiles.Add(projectile);
				Cooldown.SetMax();
			}
		}
	}
}
