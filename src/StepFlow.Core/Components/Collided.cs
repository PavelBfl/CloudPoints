﻿using System;
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

		public ShapeBase? GetShape() => PropertyName switch
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

			Current = ShapeBase.Create(Context.IntersectionContext, original.Current);
			Next = ShapeBase.Create(Context.IntersectionContext, original.Next);

			Offset = original.Offset;
			IsMove = original.IsMove;
			IsRigid = original.IsRigid;
		}

		private ShapeBase? current;

		public ShapeBase? Current { get => current; set => SetShape(ref current, value, nameof(Current)); }

		private ShapeBase? next;

		public ShapeBase? Next { get => next; set => SetShape(ref next, value, nameof(Next)); }

		private void SetShape(ref ShapeBase? shape, ShapeBase? newShape, string propertyName)
		{
			if (newShape is { } && newShape.Context != Context.IntersectionContext)
			{
				throw ExceptionBuilder.CreateUnknownIntersectionContext();
			}

			if (newShape?.Attached is { })
			{
				throw ExceptionBuilder.CreateShapeHasOwner();
			}

			if (shape is { })
			{
				shape.Disable();
				shape.Attached = null;
			}

			shape = newShape;

			if (shape is { })
			{
				shape.Attached = new CollidedAttached(propertyName, this);
				if (IsEnable)
				{
					shape.Enable();
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
				Current?.Enable();
				Next?.Enable();
				IsEnable = true;
			}
		}

		public void Disable()
		{
			if (IsEnable)
			{
				Current?.Disable();
				Next?.Disable(); 
				IsEnable = false;
			}
		}
	}
}
