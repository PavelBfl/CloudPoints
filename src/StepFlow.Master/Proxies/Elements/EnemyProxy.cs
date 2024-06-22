using System;
using System.Drawing;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Intersection;
using StepFlow.Master.Proxies.Schedulers;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IEnemyProxy : IMaterialProxy<Enemy>
	{
		Collided Vision { get; }
	}

	internal sealed class EnemyProxy : MaterialProxy<Enemy>, IEnemyProxy
	{
		public EnemyProxy(PlayMaster owner, Enemy target) : base(owner, target)
		{
		}

		public Collided Vision => Target.GetVisionRequired();

		public Scale Cooldown => Target.GetCooldownRequired();

		public override void OnTick()
		{
			base.OnTick();

			if (Strength?.Value == 0)
			{
				var items = Owner.CreateCollectionUsedProxy(Owner.Playground.Items);
				items.Remove(Target);

				var itemPosition = Body.Current.Bounds.GetCenter();
				Owner.CreateItem.Execute(new Scripts.CreateItem.Parameters()
				{
					Position = itemPosition,
					Kind = Target.ReleaseItem
				});

				var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
				bodyProxy.Clear();
				
				var visionProxy = (ICollidedProxy)Owner.CreateProxy(Vision);
				visionProxy.Clear();
			}
			else
			{
				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.Decrement();

				var center = Body.Current.Bounds.GetCenter();
				var visionPlace = RectangleExtensions.Create(center, 100);
				var visionProxy = (IShapeContainerProxy)Owner.CreateProxy(Vision.Current);
				visionProxy.Reset(visionPlace);

				if (Cooldown.Value == 0)
				{
					switch (Target.AttackStrategy)
					{
						case AttackStrategy.Left:
							CreateProjectile(new Vector2(-1, 0));
							break;
						case AttackStrategy.Top:
							CreateProjectile(new Vector2(0, -1));
							break;
						case AttackStrategy.Right:
							CreateProjectile(new Vector2(1, 0));
							break;
						case AttackStrategy.Bottom:
							CreateProjectile(new Vector2(0, 1));
							break;
					}
				}
			}
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
				else if (otherCollided.Collided.IsRigid &&
					thisCollided.Collided == Body &&
					Target.GetCourseVector(Material.SHEDULER_CONTROL_NAME) is { } path &&
					path.Runner.Current is { })
				{
					switch (Target.Strategy)
					{
						case Strategy.None:
							StrategyNone(path);
							break;
						case Strategy.CW:
							StrategyClock(true, path);
							break;
						case Strategy.CWW:
							StrategyClock(false, path);
							break;
						case Strategy.Reflection:
							StrategyReflection(thisCollided.Collided.Current, otherCollided.Collided.Current, path);
							break;
						default: throw EnumNotSupportedException.Create(Target.Strategy);
					}
				}
			}
		}

		private void StrategyNone(Material.CourseVectorPath controlVector)
		{
			var runnerProxy = (ISchedulerRunnerProxy)Owner.CreateProxy(controlVector.Runner);
			runnerProxy.Current = null;

			var controlVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(controlVector.CourseVector);
			controlVectorProxy.Value = Vector2.Zero;
		}

		private void StrategyClock(bool cw, Material.CourseVectorPath controlVector)
		{
			var runnerProxy = (ISchedulerRunnerProxy)Owner.CreateProxy(controlVector.Runner);
			runnerProxy.Current = null;

			var rotate = Matrix3x2.CreateRotation(MathF.PI / (cw ? 2 : -2));
			var controlVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(controlVector.CourseVector);
			controlVectorProxy.Value = Vector2.Transform(controlVector.CourseVector.Value, rotate);
		}

		private void StrategyReflection(ShapeContainer shape, ShapeContainer otherShape, Material.CourseVectorPath controlVector)
		{
			Vector2 newVector = controlVector.CourseVector.Value;
			if (shape.Bounds.Right <= otherShape.Bounds.Left || shape.Bounds.Left >= otherShape.Bounds.Right)
			{
				newVector.X = -newVector.X;
			}
			else if (shape.Bounds.Top <= otherShape.Bounds.Bottom || shape.Bounds.Bottom >= otherShape.Bounds.Top)
			{
				newVector.Y = -newVector.Y;
			}
			else
			{
				throw new InvalidOperationException();
			}

			var runnerProxy = (ISchedulerRunnerProxy)Owner.CreateProxy(controlVector.Runner);
			runnerProxy.Current = null;

			var controlVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(controlVector.CourseVector);
			controlVectorProxy.Value = newVector;
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

			if (Cooldown.Value == 0)
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

				var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
				cooldownProxy.SetMax();
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

			var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
			cooldownProxy.SetMax();
		}

		public override void Begin()
		{
			base.Begin();

			var visionProxy = (ICollidedProxy)Owner.CreateProxy(Vision);
			visionProxy.Register();
		}

		public override void End()
		{
			base.End();

			var visionProxy = (ICollidedProxy)Owner.CreateProxy(Vision);
			visionProxy.Unregister();
		}
	}
}
