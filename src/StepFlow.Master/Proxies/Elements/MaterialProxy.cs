using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.States;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>, IPlaygroundUsed
		where TTarget : Material
	{
		Scale Strength { get; set; }

		Collided Body { get; }

		int Speed { get; set; }

		int Weight { get; set; }

		Vector2 Course { get; set; }

		void OnTick();

		void SetCourse(float? radian);


		void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided);

		void ChangeStrength(Damage damage);
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy<TTarget>
		where TTarget : Material
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public Scale Strength { get => Target.Strength; set => SetValue(value); }

		public virtual void ChangeStrength(Damage damage)
		{
			Strength -= damage.Value;
		}

		public Collided Body { get => Target.GetBodyRequired(); }

		public virtual void OnTick()
		{
			var statesRemoved = new List<State>();
			var statesAdded = new List<State>();

			var additionalCourse = Vector2.Zero;
			foreach (var state in Target.States.Where(x => x.Enable))
			{
				var stateProxy = (IStateProxy<State>)Owner.CreateProxy(state);
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
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy<State>)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = default;
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;
						}
						break;
					case StateKind.EnemySerpentineForwardToBackward:
						if (stateProxy.Cooldown.IsMin())
						{
							stateProxy.Enable = false;

							var enemySerpentineForwardToBackwardState = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineBackwardState);
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy<State>)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = enemySerpentineForwardToBackwardStateProxy.Cooldown.SetMax();
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;
						}
						break;
					case StateKind.EnemySerpentineBackwardState:
						if (stateProxy.Cooldown.IsMin())
						{
							stateProxy.Enable = false;

							var enemySerpentineForwardToBackwardState = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineBackwardToForward);
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy<State>)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = default;
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;
						}
						break;
					case StateKind.EnemySerpentineBackwardToForward:
						if (stateProxy.Cooldown.IsMin())
						{
							stateProxy.Enable = false;

							var enemySerpentineForwardToBackwardState = Target.States.Single(x => x.Kind == StateKind.EnemySerpentineForwardState);
							var enemySerpentineForwardToBackwardStateProxy = (IStateProxy<State>)Owner.CreateProxy(enemySerpentineForwardToBackwardState);
							enemySerpentineForwardToBackwardStateProxy.Enable = true;
							enemySerpentineForwardToBackwardStateProxy.Cooldown = enemySerpentineForwardToBackwardStateProxy.Cooldown.SetMax();
							Course = enemySerpentineForwardToBackwardStateProxy.Vector;
						}
						break;
					case StateKind.MoveReflection:
					case StateKind.MoveCW:
					case StateKind.MoveCCW:
						// Handle in Collision method
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
			}
		}

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

		public virtual void Begin()
		{
			var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
			bodyProxy.Register();
		}

		public virtual void End()
		{
			var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
			bodyProxy.Unregister();
		}

		public int Speed { get => Target.Speed; set => SetValue(value); }

		public Vector2 Course { get => Target.Course; set => SetValue(value); }

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
	}
}
