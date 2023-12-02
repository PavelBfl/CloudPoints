﻿using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IEnemyProxy : IMaterialProxy<Enemy>
	{
		ICollidedProxy Vision { get; }
	}

	internal sealed class EnemyProxy : MaterialProxy<Enemy>, IEnemyProxy
	{
		public EnemyProxy(PlayMaster owner, Enemy target) : base(owner, target)
		{
		}

		public ICollidedProxy Vision => (ICollidedProxy)Owner.CreateProxy(Target.Vision);

		public IScaleProxy Cooldown => (IScaleProxy)Owner.CreateProxy(Target.Cooldown);

		public override void OnTick()
		{
			base.OnTick();

			if (Strength?.Value == 0)
			{
				Owner.GetPlaygroundProxy().Enemies.Remove(this);
			}
			else
			{
				Cooldown.Decrement();
			}
		}

		public override void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (thisCollided.Target == Vision.Target && otherMaterial.Target == Owner.Playground.PlayerCharacter)
			{
				CreateProjectile(otherMaterial);
			}
		}

		private void CreateProjectile(IMaterialProxy<Material> other)
		{
			if (Cooldown.Value == 0)
			{
				var border = Body.Current.Border;
				var center = new Point(
					border.X + border.Width / 2,
					border.Y + border.Height / 2
				);

				var otherBorder = other.Body.Current.Border;
				var otherCenter = new Point(
					otherBorder.X + otherBorder.Width / 2,
					otherBorder.Y + otherBorder.Height / 2
				);

				const int SIZE = 10;
				var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile()
				{
					Creator = Target,
					Body = new Collided()
					{
						Current = new Cell()
						{
							Border = new Rectangle(
								center.X - SIZE / 2,
								center.Y - SIZE / 2,
								SIZE,
								SIZE
							),
						},
					},
					Damage = new Damage()
					{
						Value = 10,
					},
					Speed = 5,
				});

				foreach (var course in CourseExtensions.GetPath(center, otherCenter))
				{
					projectile.SetCourse(course);
				}

				Owner.GetPlaygroundProxy().Projectiles.Add(projectile);
				Cooldown.SetMax();
			}
		}
	}
}
