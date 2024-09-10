using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core.Exceptions;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Intersection;

namespace StepFlow.Core.Components
{
	public sealed class CollidedAttached
	{
		public CollidedAttached(string propertyName, Collided collided)
		{
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
			Collided = collided ?? throw new ArgumentNullException(nameof(collided));
		}

		public string PropertyName { get; }

		public Collided Collided { get; }

		public Shape? GetShape() => PropertyName switch
		{
			nameof(Collided.Current) => Collided.Current,
			nameof(Collided.Next) => Collided.Next,
			_ => throw new InvalidOperationException(),
		};

		public override string ToString() => PropertyName + ": " + GetShape();
	}

	public sealed class Collided : ComponentBase, IEnabled
	{
		public Collided(IContext context)
			: base(context)
		{
		}

		public Collided(IContext context, CollidedDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfContextNull(context);
			CopyExtensions.ThrowIfOriginalNull(original);

			Current = Shape.Create(original.Current);
			Next = Shape.Create(original.Next);

			Offset = original.Offset;
			IsMove = original.IsMove;
			IsRigid = original.IsRigid;
		}

		private Shape? current;

		public Shape? Current { get => current; set => SetShape(ref current, value, nameof(Current)); }

		private Shape? next;

		public Shape? Next { get => next; set => SetShape(ref next, value, nameof(Next)); }

		private void SetShape(ref Shape? shape, Shape? newShape, string propertyName)
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
				shape.State = new CollidedAttached(propertyName, this);
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

		public void CopyTo(CollidedDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Current.AddRange(Current ?? Enumerable.Empty<Rectangle>());
			container.Next.AddRange(Next ?? Enumerable.Empty<Rectangle>());
			container.Offset = Offset;
			container.IsMove = IsMove;
			container.IsRigid = IsRigid;
		}

		public override SubjectDto ToDto()
		{
			var result = new CollidedDto();
			CopyTo(result);
			return result;
		}

		public bool IsEnable { get; private set; }

		public void Enable()
		{
			if (!IsEnable)
			{
				if (Current is { } current)
				{
					Context.IntersectionContext.Add(current);
				}

				if (Next is { } next)
				{
					Context.IntersectionContext.Add(next);
				}

				IsEnable = true;
			}
		}

		public void Disable()
		{
			if (IsEnable)
			{
				current?.Disable();
				next?.Disable();
				IsEnable = false;
			}
		}
	}
}
