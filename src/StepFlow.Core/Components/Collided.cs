using System;
using System.Drawing;
using System.Numerics;
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

		public ShapeContainer GetShape() => PropertyName switch
		{
			nameof(Collided.Current) => Collided.Current,
			nameof(Collided.Next) => Collided.Next,
			_ => throw new InvalidOperationException(),
		};

		public override string ToString() => PropertyName + ": " + GetShape();
	}

	public sealed class Collided : ComponentBase
	{
		public Collided(IContext context)
			: base(context)
		{
			Current = new ShapeContainer(Context.IntersectionContext);
			Next = new ShapeContainer(Context.IntersectionContext);

			Current.Attached = new CollidedAttached(nameof(Current), this);
			Next.Attached = new CollidedAttached(nameof(Next), this);
		}

		public Collided(IContext context, CollidedDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfContextNull(context);
			CopyExtensions.ThrowIfOriginalNull(original);

			Current = new ShapeContainer(Context.IntersectionContext);
			Next = new ShapeContainer(Context.IntersectionContext);

			Current.Attached = new CollidedAttached(nameof(Current), this);
			Next.Attached = new CollidedAttached(nameof(Next), this);

			Current.Add(original.Current);
			Next.Add(original.Next);
		}

		public ShapeContainer Current { get; }

		public ShapeContainer Next { get; }

		public Vector2 Position { get; set; }

		public Point PositionAsLocation => new Point(
			(int)MathF.Floor(Position.X),
			(int)MathF.Floor(Position.Y)
		);

		public void PositionSync()
		{
			var location = Current.Bounds.Location;
			Position = new Vector2(location.X, location.Y);
		}

		public bool IsMove { get; set; }

		public bool IsRigid { get; set; }

		public void CopyTo(CollidedDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Current.AddRange(Current);
			container.Next.AddRange(Next);
			container.Position = Position;
			container.IsMove = IsMove;
			container.IsRigid = IsRigid;
		}

		public override SubjectDto ToDto()
		{
			var result = new CollidedDto();
			CopyTo(result);
			return result;
		}
	}
}
