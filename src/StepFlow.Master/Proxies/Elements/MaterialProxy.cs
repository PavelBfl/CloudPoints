using System;
using System.Collections.Generic;
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

		Vector2 Course { get; set; }

		ICollection<State> States { get; }

		CollisionBehavior CollisionBehavior { get; set; }

		TrackBuilder? Track { get; set; }

		void OnTick();

		void SetCourse(float? radian);

		void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided);

		void CopyFrom(MaterialDto original)
		{
			NullValidateExtensions.ThrowIfArgumentNull(original, nameof(original));

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
			bodyProxy.SetPosition(bodyProxy.Position + Course + additionalCourse, true);
		}

		public virtual void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (Target != otherMaterial && otherCollided.Collided.IsRigid && thisCollided.Collided.IsRigid)
			{
				((ICollidedProxy)Owner.CreateProxy(Body)).Break();

				switch (CollisionBehavior)
				{
					case CollisionBehavior.None:
						break;
					case CollisionBehavior.Stop:
						StrategyNone();
						break;
					case CollisionBehavior.Reflection:
						StrategyReflection(thisCollided.Collided.Current, otherCollided.Collided.Current);
						break;
					case CollisionBehavior.CW:
						StrategyClock(true);
						break;
					case CollisionBehavior.CCW:
						StrategyClock(false);
						break;
					default: throw EnumNotSupportedException.Create(CollisionBehavior);
				}
			}
		}

		private void StrategyNone()
		{
			Course = Vector2.Zero;
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

		private static void Negative(ref float value) => value = value <= 0 ? value : -value;

		public void SetCourse(float? radians)
		{
			Course = Vector2.Transform(
				new Vector2(radians is { } ? 0.05f : 0, 0),
				Matrix3x2.CreateRotation(radians ?? 0)
			);
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

		public ICollection<State> States => Target.States;

		public TrackBuilder? Track { get => Target.Track; set => SetValue(value); }
	}
}
