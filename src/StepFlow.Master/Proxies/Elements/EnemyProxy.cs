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
	public interface IEnemyProxy : IProxyBase<Enemy>, IMaterialProxy
	{
		ICollidedProxy Vision { get; }
	}

	internal sealed class EnemyProxy : MaterialProxy<Enemy>, IEnemyProxy
	{
		public EnemyProxy(PlayMaster owner, Enemy target) : base(owner, target)
		{
		}

		public ICollidedProxy Vision => new VisionProxy(Owner, Target.Vision, this);

		public IScaleProxy Cooldown => (IScaleProxy)Owner.CreateProxy(Target.Cooldown);

		public override void OnTick()
		{
			base.OnTick();

			Cooldown.Decrement();
		}

		private void VisionCollision(ICollidedProxy other)
		{
			if (Cooldown.Value == 0)
			{
				var border = Current.Border;
				var center = new Point(
					border.X + border.Width / 2,
					border.Y + border.Height / 2
				);

				var otherBorder = other.Current.Border;
				var otherCenter = new Point(
					otherBorder.X + otherBorder.Width / 2,
					otherBorder.Y + otherBorder.Height / 2
				);

				const int SIZE = 10;
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

				foreach (var course in CourseExtensions.GetPath(center, otherCenter))
				{
					projectile.SetCourse(course);
				}

				Owner.GetPlaygroundProxy().Projectiles.Add(projectile);
				Cooldown.SetMax();
			}
		}

		private sealed class VisionProxy : ProxyBase<ICollided>, ICollidedProxy
		{
			public VisionProxy(PlayMaster owner, ICollided target, EnemyProxy enemy) : base(owner, target)
			{
				Enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
			}

			public EnemyProxy Enemy { get; }

			public IBorderedBaseProxy<IBordered>? Current
			{
				get => (IBorderedBaseProxy<IBordered>?)Owner.CreateProxy(Target.Current);
				set => SetValue(x => x.Current, value?.Target);
			}

			public IBorderedBaseProxy<IBordered>? Next
			{
				get => (IBorderedBaseProxy<IBordered>?)Owner.CreateProxy(Target.Next);
				set => SetValue(x => x.Next, value?.Target);
			}
			public bool IsMove { get => Target.IsMove; set => SetValue(x => x.IsMove, value); }

			public void Collision(ICollidedProxy other) => Enemy.VisionCollision(other);
		}
	}
}
