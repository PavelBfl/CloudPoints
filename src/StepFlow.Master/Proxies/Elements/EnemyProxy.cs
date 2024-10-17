using System;
using System.Drawing;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IEnemyProxy : IMaterialProxy<Enemy>
	{
		Shape? Vision { get; set; }
		Scale StunCooldown { get; set; }
	}

	internal sealed class EnemyProxy : MaterialProxy<Enemy>, IEnemyProxy
	{
		public EnemyProxy(PlayMaster owner, Enemy target) : base(owner, target)
		{
		}

		public Shape? Vision { get => Target.Vision; set => SetValue(value); }

		public Scale Cooldown { get => Target.Cooldown; set => SetValue(value); }

		public override Vector2 Course
		{
			get => base.Course;
			set
			{
				if (StunCooldown.Value == 0)
				{
					base.Course = new Vector2(Target.PatrolSpeed ?? value.X, value.Y);
				}
				else
				{
					base.Course = value;
				}
			}
		}

		public float? PatrolSpeed { get => Target.PatrolSpeed; set => SetValue(value); }

		public Scale StunCooldown { get => Target.StunCooldown; set => SetValue(value); }

		public override void OnTick()
		{
			base.OnTick();

			if (Strength.IsMin())
			{
				var items = Owner.CreateCollectionProxy(Owner.Playground.Items);
				items.Remove(Target);

				var itemPosition = Body.GetCurrentRequired().Bounds.GetCenter();
				Owner.CreateItem.Execute(new Scripts.CreateItem.Parameters()
				{
					Position = itemPosition,
					Kind = Target.ReleaseItem
				});

				var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
				bodyProxy.Clear();

				Vision = null;
			}
			else
			{
				Cooldown--;
				StunCooldown--;

				if (StunCooldown.Value == 0)
				{
					var center = Body.GetCurrentRequired().Bounds.GetCenter();
					var visionPlace = RectangleExtensions.Create(center, 100);
					Vision = Shape.Create(new[] { visionPlace });

					if (Vision is { } vision)
					{
						foreach (var otherShape in vision.GetCollisions())
						{
							var otherAttached = (CollidedAttached)NullValidate.PropertyRequired(otherShape.State, nameof(Shape.State));

							if (otherAttached.Material is PlayerCharacter playerCharacter)
							{
								CreateProjectile(playerCharacter);
							}
						}
					}

					if (PatrolSpeed is { } patrolSpeed)
					{
						if (RigidExists(new Point(1, 0)))
						{
							PatrolSpeed = -MathF.Abs(patrolSpeed);
						}
						else if (RigidExists(new Point(-1, 0)))
						{
							PatrolSpeed = MathF.Abs(patrolSpeed);
						}
					} 
				}
			}
		}

		private static Vector2 GetCenter(Material material)
		{
			var bounds = material.Body.GetCurrentRequired().Bounds;

			return new Vector2(
				bounds.X + bounds.Width / 2f,
				bounds.Y + bounds.Height / 2f
			);
		}

		private void CreateProjectile(Material other)
		{
			const int SIZE = 10;

			if (Cooldown.IsMin())
			{
				var center = GetCenter(Target);
				var otherCenter = GetCenter(other);
				var course = otherCenter - center;
				course = Vector2.Normalize(course) * 0.05f;

				Owner.CreateProjectile(
					Body.GetCurrentRequired().Bounds.GetCenter(),
					SIZE,
					course,
					new Damage() { Value = 10, },
					TimeTick.FromSeconds(2),
					Target,
					ReusableKind.None
				);

				Cooldown = Cooldown.SetMax();
			}
		}

		private void CreateProjectile(Vector2 course)
		{
			const int SIZE = 10;

			course = Vector2.Normalize(course) * 0.05f;

			Owner.CreateProjectile(
				Body.GetCurrentRequired().Bounds.GetCenter(),
				SIZE,
				course,
				new Damage() { Value = 10, },
				TimeTick.FromSeconds(2),
				Target,
				ReusableKind.None
			);

			Cooldown = Cooldown.SetMax();
		}
	}
}
