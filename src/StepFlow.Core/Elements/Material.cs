using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core.Exceptions;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;

namespace StepFlow.Core.Elements
{
	public abstract class Material : ElementBase, IEnabled
	{
		public const string SHEDULER_CONTROL_NAME = "Control";
		public const string SHEDULER_INERTIA_NAME = "Inertia";

		public const int MAX_WEIGHT = 100000;

		public Material(IContext context)
			: base(context)
		{
			Body = new Collided(this);
		}

		public Material(IContext context, MaterialDto original)
			: base(context, original)
		{
			Body = new Collided(this)
			{
				Current = Shape.Create(original.BodyCurrent),
				Next = Shape.Create(original.BodyNext),
				IsMove = original.BodyIsMove,
				IsRigid = original.IsRigid,
				Offset = original.BodyOffset,
			};

			Strength = original.Strength;
			Speed = original.Speed;
			Weight = original.Weight;
			Elasticity = original.Elasticity;
			Course = original.Course;
			IsFixed = original.IsFixed;
			States.AddUniqueRange(original.States.Select(x => x.ToState(Context)));
			Route = original.Route?.ToRoute(context);
			Track = original.Track?.ToTrackBuilder(Context);
		}

		protected void SetShape(ref Shape? shape, Shape? newShape, string name)
		{
			if (newShape?.State is { })
			{
				throw ExceptionBuilder.CreateShapeHasOwner();
			}

			if (shape is { })
			{
				shape.Disable();
				shape.State = null;
			}

			if (newShape is { })
			{
				shape = newShape;
				shape.State = new CollidedAttached(name, this);
			}
			else
			{
				shape = null;
			}

			if (shape is { })
			{
				if (IsEnable)
				{
					Context.IntersectionContext.Add(shape);
				}
				else
				{
					shape.Disable();
				}
			}
		}

		public virtual Shape? GetShape(string name)
		{
			NullValidate.ThrowIfArgumentNull(name, nameof(name));

			return name switch
			{
				nameof(Collided.Current) => Body.Current,
				nameof(Collided.Next) => Body.Next,
				_ => null,
			};
		}

		public Scale Strength { get; set; }

		public Collided Body { get; }

		public int Speed { get; set; }

		public int Weight { get; set; }

		public float Elasticity { get; set; }

		public Vector2 Course { get; set; }

		public bool IsFixed { get; set; }

		public ICollection<State> States { get; } = new HashSet<State>();

		public Route? Route { get; set; }

		public TrackBuilder? Track { get; set; }

		public void CopyTo(MaterialDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			Body.CopyTo(container);

			container.Strength = Strength;
			container.Speed = Speed;
			container.Weight = Weight;
			container.Elasticity = Elasticity;
			container.Course = Course;
		}

		public bool IsEnable { get; private set; }

		public virtual void Enable()
		{
			Body.Enable();
			IsEnable = true;
		}

		public virtual void Disable()
		{
			Body.Disable();
			IsEnable = false;
		}

		public sealed class Collided
		{
			public Collided(Material owner)
			{
				NullValidate.ThrowIfArgumentNull(owner, nameof(owner));

				Owner = owner;
			}

			public Material Owner { get; }

			private Shape? current;

			public Shape? Current { get => current; set => Owner.SetShape(ref current, value, nameof(Current)); }

			public Shape GetCurrentRequired() => NullValidate.PropertyRequired(Current, nameof(Current));

			private Shape? next;

			public Shape? Next { get => next; set => Owner.SetShape(ref next, value, nameof(Next)); }

			public Shape GetNextRequired() => NullValidate.PropertyRequired(Next, nameof(Next));

			public Vector2 Offset { get; set; }

			public bool IsMove { get; set; }

			public bool IsRigid { get; set; }

			public Point? GetOffset()
			{
				if (!IsMove && Current is { })
				{
					return Point.Empty;
				}

				if (Current is null || Next is null)
				{
					return null;
				}

				if (Current.Count != Next.Count)
				{
					return null;
				}

				var sourceBounds = Current.Bounds;
				var otherBounds = Next.Bounds;

				if (sourceBounds.Width != otherBounds.Width || sourceBounds.Height != otherBounds.Height)
				{
					return null;
				}

				var result = new Point(
					otherBounds.X - sourceBounds.X,
					otherBounds.Y - sourceBounds.Y
				);

				foreach (var sourceOffset in Current.AsEnumerable().Offset(result))
				{
					if (!Next.Contains(sourceOffset))
					{
						return null;
					}
				}

				return result;
			}

			public Point? GetAggregateOffset(Collided other)
			{
				if (GetOffset() is { } thisOffset && other.GetOffset() is { } otherOffset)
				{
					return new Point(
						thisOffset.X - otherOffset.X,
						thisOffset.Y - otherOffset.Y
					);
				}
				else
				{
					return null;
				}
			}

			public void CopyTo(MaterialDto material)
			{
				CopyExtensions.ThrowIfArgumentNull(material, nameof(material));

				material.BodyCurrent.Reset(Current);
				material.BodyNext.Reset(Next);
				material.BodyOffset = Offset;
				material.BodyIsMove = IsMove;
				material.IsRigid = IsRigid;
			}

			public void Enable()
			{
				if (!Owner.IsEnable)
				{
					if (Current is { } current)
					{
						Owner.Context.IntersectionContext.Add(current);
					}

					if (Next is { } next)
					{
						Owner.Context.IntersectionContext.Add(next);
					}
				}
			}

			public void Disable()
			{
				if (Owner.IsEnable)
				{
					current?.Disable();
					next?.Disable();
				}
			}
		}
	}
}
