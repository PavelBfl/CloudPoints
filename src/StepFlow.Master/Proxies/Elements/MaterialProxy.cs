using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.States;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : Material
	{
		Scale Strength { get; set; }

		Collided Body { get; }

		int Speed { get; set; }

		int Weight { get; set; }

		float Elasticity { get; set; }

		Vector2 Course { get; set; }

		ICollection<State> States { get; }

		CollisionBehavior CollisionBehavior { get; set; }

		TrackBuilder? Track { get; set; }

		void OnTick();

		void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided);

		void CopyFrom(MaterialDto original)
		{
			NullValidate.ThrowIfArgumentNull(original, nameof(original));

			Strength = original.Strength;
			var collidedProxy = ((ICollidedProxy)Owner.CreateProxy(Body));
			if (original.Body is { })
			{
				collidedProxy.CopyFrom(original.Body);
			}
			else
			{
				collidedProxy.Clear();
			}
			Speed = original.Speed;
			Weight = original.Weight;
			Course = original.Course;
			CollisionBehavior = original.CollisionBehavior;
			var statesProxy = Owner.CreateCollectionProxy(States);
			statesProxy.Clear();
			foreach (var state in original.States)
			{
				statesProxy.Add(new State(Owner.Playground.Context, state));
			}
			Track = original.Track?.ToTrackBuilder(Owner.Playground.Context);
		}
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy<TTarget>
		where TTarget : Material
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public Scale Strength { get => Target.Strength; set => SetValue(value); }

		public Collided Body { get => Target.GetBodyRequired(); }

		public virtual void OnTick()
		{
			var statesRemoved = new List<State>();
			var statesAdded = new List<State>();

			var additionalCourse = Vector2.Zero;
			foreach (var state in Target.States.Where(x => x.Enable))
			{
				var stateProxy = (IStateProxy)Owner.CreateProxy(state);
				stateProxy.Cooldown--;
				stateProxy.TotalCooldown--;

				switch (state.Kind)
				{
					case StateKind.Remove:
						if (state.TotalCooldown == 0)
						{
							Owner.GetPlaygroundItemsProxy().Remove(Target);
						}
						break;
					case StateKind.Poison:
						Strength--;
						break;
					case StateKind.Arc:
						Course = Vector2.Transform(
							Course,
							Matrix3x2.CreateRotation(MathF.PI / 2400)
						);
						break;
					case StateKind.Push:
						var pushVector = stateProxy.Vector;
						additionalCourse += pushVector;
						var factor = 1f - (Weight / (float)Material.MAX_WEIGHT);
						stateProxy.Vector = Vector2.Transform(
							pushVector,
							Matrix3x2.CreateScale(factor)
						);
						break;
					case StateKind.Dash:
						additionalCourse += stateProxy.Vector;
						break;
					case StateKind.CreatingProjectile:
						if (state.Cooldown.IsMin())
						{
							CreateProjectile(stateProxy.Arg0);
						}
						break;
					case StateKind.Gravity:
						Course += stateProxy.Vector;
						break;
					case StateKind.EnemySerpentineForwardState:
						if (stateProxy.Cooldown.IsMin())
						{
							stateProxy.Enable = false;

							var enemySerpentineForwardToBackwardState = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineForwardToBackward);
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = default;
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;

							var enemySerpentineForwardStateAttack = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineForwardStateAttack);
							var enemySerpentineForwardStateAttackProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineForwardStateAttack);
							enemySerpentineForwardStateAttackProxy.Enable = false;

							CollisionBehavior = CollisionBehavior.Stop;
						}
						break;
					case StateKind.EnemySerpentineForwardStateAttack:
						if (stateProxy.Cooldown.IsMin())
						{
							CreateProjectile(stateProxy.Arg0);
						}
						break;
					case StateKind.EnemySerpentineForwardToBackward:
						if (Course == Vector2.Zero)
						{
							stateProxy.Enable = false;

							var enemySerpentineForwardToBackwardState = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineBackwardState);
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = enemySerpentineForwardToBackwardStateProxy.Cooldown.SetMax();
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;

							var enemySerpentineBackwardStateAttack = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineBackwardStateAttack);
							var enemySerpentineBackwardStateAttackProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineBackwardStateAttack);
							enemySerpentineBackwardStateAttackProxy.Enable = true;

							CollisionBehavior = CollisionBehavior.Reflection;
						}
						break;
					case StateKind.EnemySerpentineBackwardState:
						if (stateProxy.Cooldown.IsMin())
						{
							stateProxy.Enable = false;

							var enemySerpentineForwardToBackwardState = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineBackwardToForward);
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = default;
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;

							var enemySerpentineBackwardStateAttack = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineBackwardStateAttack);
							var enemySerpentineBackwardStateAttackProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineBackwardStateAttack);
							enemySerpentineBackwardStateAttackProxy.Enable = false;

							CollisionBehavior = CollisionBehavior.Stop;
						}
						break;
					case StateKind.EnemySerpentineBackwardStateAttack:
						if (stateProxy.Cooldown.IsMin())
						{
							CreateProjectile(stateProxy.Arg0);
						}
						break;
					case StateKind.EnemySerpentineBackwardToForward:
						if (Course == Vector2.Zero)
						{
							stateProxy.Enable = false;

							var enemySerpentineForwardToBackwardState = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineForwardState);
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = enemySerpentineForwardToBackwardStateProxy.Cooldown.SetMax();
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;

							var enemySerpentineForwardStateAttack = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineForwardStateAttack);
							var enemySerpentineForwardStateAttackProxy = (IStateProxy)Owner.CreateProxy(enemySerpentineForwardStateAttack);
							enemySerpentineForwardStateAttackProxy.Enable = true;

							CollisionBehavior = CollisionBehavior.Reflection;
						}
						break;
					default: throw EnumNotSupportedException.Create(state.Kind);
				}

				if (state.TotalCooldown == 0)
				{
					statesRemoved.Add(state);
				}
				else if (state.Cooldown.IsMin())
				{
					stateProxy.Cooldown = state.Cooldown.SetMax();
				}
			}

			if (statesRemoved.Count > 0)
			{
				var statesProxy = Owner.CreateCollectionProxy(Target.States);
				foreach (var state in statesRemoved)
				{
					statesProxy.Remove(state);
				}
			}

			if (statesAdded.Count > 0)
			{
				var statesProxy = Owner.CreateCollectionProxy(Target.States);
				foreach (var state in statesAdded)
				{
					statesProxy.Add(state);
				}
			}

			var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
			bodyProxy.Offset += Course + additionalCourse;
			bodyProxy.SetOffset();
		}

		public virtual void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (Target != otherMaterial && otherCollided.Collided.IsRigid && thisCollided.Collided.IsRigid)
			{
				switch (CollisionBehavior)
				{
					case CollisionBehavior.None:
						break;
					case CollisionBehavior.Stop:
						Course = Vector2.Zero;
						break;
					case CollisionBehavior.Reflection:
						if (Body.GetOffset() is { } sourceOffset && otherCollided.Collided.GetOffset() is { } otherOffset)
						{
							var aggregateOffset = new Point(
								sourceOffset.X - otherOffset.X,
								sourceOffset.Y - otherOffset.Y
							);

							if (aggregateOffset.ToCourse() is { } sourceCourse)
							{
								var isHorizontal = sourceCourse == Common.Course.Top ||
									sourceCourse == Common.Course.RightTop ||
									sourceCourse == Common.Course.Bottom ||
									sourceCourse == Common.Course.LeftBottom;

								var factor = (Elasticity + otherMaterial.Elasticity) / 2;

								if (isHorizontal)
								{
									(var u1, var u2) = Collision(
										Target.Weight,
										otherMaterial.Weight,
										Target.Course.Y,
										otherMaterial.Course.Y
									);

									Course = new Vector2(Course.X, u1) * factor;
									otherMaterial.Course = new Vector2(otherMaterial.Course.X, u2) * factor;
								}
								else
								{
									(var u1, var u2) = Collision(
										Target.Weight,
										otherMaterial.Weight,
										Target.Course.X,
										otherMaterial.Course.X
									);

									Course = new Vector2(u1, Course.Y) * factor;
									otherMaterial.Course = new Vector2(u2, otherMaterial.Course.Y) * factor;
								}
							}
						}
						break;
					case CollisionBehavior.CW:
						StrategyClock(true);
						break;
					case CollisionBehavior.CCW:
						StrategyClock(false);
						break;
					default: throw EnumNotSupportedException.Create(CollisionBehavior);
				}

				((ICollidedProxy)Owner.CreateProxy(Body)).Break();
			}
		}

		private static (float u1, float u2) Collision(float m1, float m2, float u1, float u2)
		{
			return (
				((m1 - m2) * u1 + 2 * m2 * u2) / (m1 + m2),
				((m2 - m1) * u2 + 2 * m1 * u1) / (m1 + m2)
			);
		}

		private void StrategyClock(bool cw)
		{
			var rotate = Matrix3x2.CreateRotation(MathF.PI / (cw ? 2 : -2));
			Course = Vector2.Transform(Course, rotate);
		}

		private void StrategyReflection(ShapeContainer shape, ShapeContainer otherShape)
		{
			var newCourse = Course;
			if (shape.Bounds.Right <= otherShape.Bounds.Left)
			{
				Negative(ref newCourse.X);
			}
			else if (shape.Bounds.Left >= otherShape.Bounds.Right)
			{
				Positive(ref newCourse.X);
			}
			else if (shape.Bounds.Top >= otherShape.Bounds.Bottom)
			{
				Positive(ref newCourse.Y);
			}
			else if (shape.Bounds.Bottom <= otherShape.Bounds.Top)
			{
				Negative(ref newCourse.Y);
			}
			else
			{
				throw new InvalidOperationException();
			}

			Course = newCourse;
		}

		private static void Positive(ref float value) => value = value >= 0 ? value : -value;

		private static float Positive(float value) => value >= 0 ? value : -value;

		private static void Negative(ref float value) => value = value <= 0 ? value : -value;

		private static float Negative(float value) => value <= 0 ? value : -value;

		private static Course? CollisionPoint(ShapeBase source, ShapeBase other, Course sourceCourse)
		{
			switch (sourceCourse)
			{
				case Common.Course.Left:
					return Common.Course.Left;
				case Common.Course.LeftTop:
					return (IsCollision(source, other, Common.Course.Right), IsCollision(source, other, Common.Course.Bottom)) switch
					{
						(true, false) => Common.Course.Top,
						(false, true) => Common.Course.Left,
						_ => Common.Course.LeftTop,
					};
				case Common.Course.Top:
					return Common.Course.Top;
				case Common.Course.RightTop:
					return (IsCollision(source, other, Common.Course.Left), IsCollision(source, other, Common.Course.Bottom)) switch
					{
						(true, false) => Common.Course.Top,
						(false, true) => Common.Course.Right,
						_ => Common.Course.RightTop,
					};
				case Common.Course.Right:
					return Common.Course.Right;
				case Common.Course.RightBottom:
					return (IsCollision(source, other, Common.Course.Left), IsCollision(source, other, Common.Course.Top)) switch
					{
						(true, false) => Common.Course.Bottom,
						(false, true) => Common.Course.Right,
						_ => Common.Course.RightBottom,
					};
				case Common.Course.Bottom:
					return Common.Course.Bottom;
				case Common.Course.LeftBottom:
					return (IsCollision(source, other, Common.Course.Right), IsCollision(source, other, Common.Course.Top)) switch
					{
						(true, false) => Common.Course.Bottom,
						(false, true) => Common.Course.Left,
						_ => Common.Course.LeftBottom,
					};
				default: throw EnumNotSupportedException.Create(sourceCourse);
			}
		}

		private static bool IsCollision(ShapeBase source, ShapeBase other, Course sourceOffset)
		{
			var offset = sourceOffset.ToPoint();

			var sourceBounds = source.Bounds;
			sourceBounds.Offset(offset);

			if (other.Bounds.IntersectsWith(sourceBounds))
			{
				foreach (var sourceRectangle in source)
				{
					sourceRectangle.Offset(offset);
					foreach (var otherRectangle in other)
					{
						if (sourceRectangle.IntersectsWith(otherRectangle))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		protected virtual void CreateProjectile(float radians)
		{
		}

		public int Speed { get => Target.Speed; set => SetValue(value); }

		public Vector2 Course { get => Target.Course; set => SetValue(value); }

		public CollisionBehavior CollisionBehavior { get => Target.CollisionBehavior; set => SetValue(value); }

		public int Weight
		{
			get => Target.Weight;
			set
			{
				if (value < 0 || Material.MAX_WEIGHT < value)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				SetValue(value);
			}
		}

		public float Elasticity { get => Target.Elasticity; set => SetValue(value); }

		public ICollection<State> States => Target.States;

		public TrackBuilder? Track { get => Target.Track; set => SetValue(value); }
	}
}
