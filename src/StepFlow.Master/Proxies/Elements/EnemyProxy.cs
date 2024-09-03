using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Intersection;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IEnemyProxy : IMaterialProxy<Enemy>
	{
		Collided? Vision { get; }
	}

	internal sealed class EnemyProxy : MaterialProxy<Enemy>, IEnemyProxy
	{
		public EnemyProxy(PlayMaster owner, Enemy target) : base(owner, target)
		{
		}

		public Collided? Vision => Target.Vision;

		public Scale Cooldown { get => Target.Cooldown; set => SetValue(value); }

		public override void OnTick()
		{
			base.OnTick();

			if (Strength.Value == 0)
			{
				var items = Owner.CreateCollectionProxy(Owner.Playground.Items);
				items.Remove(Target);

				var itemPosition = Body.Current.Bounds.GetCenter();
				Owner.CreateItem.Execute(new Scripts.CreateItem.Parameters()
				{
					Position = itemPosition,
					Kind = Target.ReleaseItem
				});

				var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
				bodyProxy.Clear();
				
				var visionProxy = (ICollidedProxy?)Owner.CreateProxy(Vision);
				visionProxy?.Clear();
			}
			else
			{
				Cooldown--;

				var center = Body.Current.Bounds.GetCenter();
				var visionPlace = RectangleExtensions.Create(center, 100);
				var visionProxy = (IShapeProxy?)Owner.CreateProxy(Vision?.Current);
				visionProxy?.Reset(visionPlace);
			}
		}

		protected override void CreateProjectile(float radians)
		{
			var course = Vector2.Transform(
				new Vector2(1, 0),
				Matrix3x2.CreateRotation(radians)
			);

			CreateProjectile(course);
		}

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			base.Collision(thisCollided, otherMaterial, otherCollided);

			if (Target != otherMaterial)
			{
				if (thisCollided.Collided == Vision && otherMaterial is PlayerCharacter)
				{
					CreateProjectile(otherMaterial);
				}
			}
		}

		private static Vector2 GetCenter(Material material)
		{
			var bounds = material.GetBodyRequired().Current.Bounds;

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
					Body.Current.Bounds.GetCenter(),
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
				Body.Current.Bounds.GetCenter(),
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
