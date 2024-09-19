using System.Numerics;
using StepFlow.Common;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;

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

				var itemPosition = Body.GetCurrentRequired().Bounds.GetCenter();
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

				var center = Body.GetCurrentRequired().Bounds.GetCenter();
				var visionPlace = RectangleExtensions.Create(center, 100);

				if ((ICollidedProxy?)Owner.CreateProxy(Vision) is { } visionProxy)
				{
					visionProxy.Current = Shape.Create(new[] { visionPlace });
				}
			}

			if (Vision?.Current is { } vision)
			{
				foreach (var otherShape in vision.GetCollisions())
				{
					var otherAttached = (CollidedAttached)NullValidate.PropertyRequired(otherShape.State, nameof(Shape.State));

					if (otherAttached.Collided.Element is PlayerCharacter playerCharacter)
					{
						CreateProjectile(playerCharacter);
					}
				}
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

		private static Vector2 GetCenter(Material material)
		{
			var bounds = material.GetBodyRequired().GetCurrentRequired().Bounds;

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
