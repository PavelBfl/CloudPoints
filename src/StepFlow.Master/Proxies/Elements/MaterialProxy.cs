using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
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

		Material.Collided Body { get; }

		int Speed { get; set; }

		int Weight { get; set; }

		float Elasticity { get; set; }

		Vector2 Course { get; set; }

		ICollection<State> States { get; }

		TrackBuilder? Track { get; set; }

		void OnTick();

		void CopyFrom(MaterialDto original)
		{
			NullValidate.ThrowIfArgumentNull(original, nameof(original));

			var collidedProxy = ((ICollidedProxy)Owner.CreateProxy(Body));
			collidedProxy.CopyFrom(original);

			Strength = original.Strength;
			Speed = original.Speed;
			Weight = original.Weight;
			Course = original.Course;
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

		public Material.Collided Body { get => Target.Body; }

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
						if (!RigidExists(new Point(0, 1)))
						{
							Course += stateProxy.Vector;
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

			Collision(Body.Current);
			Collision(Body.Next);
		}

		private void Collision(Shape? shape)
		{
			if (shape is { })
			{
				var thishAttached = (CollidedAttached)NullValidate.PropertyRequired(shape.State, nameof(Shape.State));
				foreach (var otherAttached in shape.GetCollisions().Select(x => x.State).OfType<CollidedAttached>())
				{
					Collision(thishAttached, otherAttached.Material, otherAttached);
				}
			}
		}

		protected virtual void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (Target != otherMaterial && otherCollided.Material.Body.IsRigid && thisCollided.Material.Body.IsRigid)
			{
				if ((!Target.IsFixed || !otherMaterial.IsFixed) && 
					Body.GetAggregateOffset(otherCollided.Material.Body) is { } aggregateOffset &&
					aggregateOffset.ToCourse() is { } sourceCourse
				)
				{
					var isVertical = sourceCourse == Common.Course.Top ||
						sourceCourse == Common.Course.RightTop ||
						sourceCourse == Common.Course.Bottom ||
						sourceCourse == Common.Course.LeftBottom;

					var factor = (Elasticity + otherMaterial.Elasticity) / 2;

					var otherMaterialProxy = (IMaterialProxy<Material>)Owner.CreateProxy(otherMaterial);
					if (isVertical)
					{
						(var u1, var u2) = Collision(
							Target.Weight,
							otherMaterial.Weight,
							Target.Course.Y,
							otherMaterial.Course.Y
						);

						if (!Target.IsFixed)
						{
							Course = new Vector2(Course.X, u1) * factor;
						}

						if (!otherMaterial.IsFixed)
						{
							otherMaterialProxy.Course = new Vector2(otherMaterial.Course.X, u2) * factor; 
						}
					}
					else
					{
						(var u1, var u2) = Collision(
							Target.Weight,
							otherMaterial.Weight,
							Target.Course.X,
							otherMaterial.Course.X
						);

						if (!Target.IsFixed)
						{
							Course = new Vector2(u1, Course.Y) * factor; 
						}

						if (!otherMaterial.IsFixed)
						{
							otherMaterialProxy.Course = new Vector2(u2, otherMaterial.Course.Y) * factor; 
						}
					}
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

		protected virtual void CreateProjectile(float radians)
		{
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

		public float Elasticity { get => Target.Elasticity; set => SetValue(value); }

		public ICollection<State> States => Target.States;

		public TrackBuilder? Track { get => Target.Track; set => SetValue(value); }

		public bool RigidExists(Point offset)
		{
			var currentBody = Body.GetCurrentRequired();
			var grip = Shape.Create(currentBody.Offset(offset));

			foreach (var intersectShape in Target.Context.IntersectionContext.GetCollisions(grip))
			{
				if (intersectShape != currentBody)
				{
					var collided = (CollidedAttached)NullValidate.PropertyRequired(intersectShape.State, nameof(Shape.State));
					if (collided.Material.Body.IsRigid && collided.Name == nameof(Material.Collided.Current))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
