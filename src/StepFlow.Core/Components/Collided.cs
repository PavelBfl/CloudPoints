using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
			nameof(Collided.Current) => Collided.CurrentShape,
			nameof(Collided.Next) => Collided.NextShape,
			_ => throw new InvalidOperationException(),
		};

		public override string ToString() => PropertyName + ": " + GetShape();
	}

	public sealed class ShapeControl : IReadOnlyList<Rectangle>
	{
		public ShapeControl(Intersection.Context context, IEnumerable<Rectangle> rectangles)
		{
			NullValidate.ThrowIfArgumentNull(context, nameof(context));

			Context = context;
			Rectangles = rectangles.ToArray();
		}

		public Intersection.Context Context { get; }

		private object? state;

		public object? State
		{
			get => state;
			set
			{
				state = value;
				if (Shape is { })
				{
					Shape.State = State;
				}
			}
		}

		private Rectangle[] Rectangles { get; set; } = Array.Empty<Rectangle>();

		public Shape? Shape { get; private set; }

		private IReadOnlyList<Rectangle> Container => (IReadOnlyList<Rectangle>?)Shape ?? Rectangles;

		public bool IsEnable => Shape is { };

		public int Count => Container.Count;

		public Rectangle this[int index] => Container[index];

		public void Enable()
		{
			if (!IsEnable)
			{
				Shape = Context.CreateShape(Rectangles);
				Shape.State = State;
				Rectangles = Array.Empty<Rectangle>();
			}
		}

		public void Disable()
		{
			if (IsEnable)
			{
				Rectangles = Shape.ToArray();
				Shape.Disable();
				Shape = null;
			}
		}

		public IEnumerator<Rectangle> GetEnumerator() => Container.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

			Current = Context.IntersectionContext.CreateShape(original.Current);
			Next = Context.IntersectionContext.CreateShape(original.Next);

			Offset = original.Offset;
			IsMove = original.IsMove;
			IsRigid = original.IsRigid;
		}

		private ShapeControl? current;

		public IEnumerable<Rectangle>? Current { get => current; set => SetShape(ref current, value, nameof(Current)); }

		public Shape? CurrentShape => current?.Shape;

		private ShapeControl? next;

		public IEnumerable<Rectangle>? Next { get => next; set => SetShape(ref next, value, nameof(Next)); }

		public Shape? NextShape => next?.Shape;

		private void SetShape(ref ShapeControl? shape, IEnumerable<Rectangle>? newShape, string propertyName)
		{
			if (shape is { })
			{
				shape.Disable();
				shape.State = null;
			}

			if (newShape is { })
			{
				shape = new ShapeControl(Context.IntersectionContext, newShape ?? Array.Empty<Rectangle>())
				{
					State = new CollidedAttached(propertyName, this),
				};
			}
			else
			{
				shape = null;
			}

			if (shape is { })
			{
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
			if (!IsMove && CurrentShape is { })
			{
				return Point.Empty;
			}

			if (CurrentShape is null || NextShape is null)
			{
				return null;
			}

			if (CurrentShape.Count != NextShape.Count)
			{
				return null;
			}

			var sourceBounds = CurrentShape.Bounds;
			var otherBounds = NextShape.Bounds;

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
				current?.Enable();
				next?.Enable();
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
