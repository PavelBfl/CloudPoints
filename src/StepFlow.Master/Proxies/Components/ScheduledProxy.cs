using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	public sealed class ScheduledProxy : ComponentProxy<Scheduled>, IScheduledProxy
	{
		public ScheduledProxy(PlayMaster owner, Scheduled target) : base(owner, target)
		{
		}

		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		private IList<Turn> Queue => new ListProxy<Turn, List<Turn>>(Owner, Target.Queue);

		public bool IsEmpty => !Queue.Any();

		public void CreateProjectile(Course course) => Add(new ProjectileBuilderTurn(this, course, 1, 10, 10));

		public void SetCourse(Course course, int stepTime = 1) => Add(CourseTurn.Create(this, course, stepTime));

		private void Add(Turn turn)
		{
			var queue = Queue;

			if (!queue.Any())
			{
				QueueBegin = Owner.Time;
			}

			queue.Add(turn);
		}

		public bool TryDequeue()
		{
			var queue = Queue;

			if (queue.Any())
			{
				var turn = queue[0];
				if (QueueBegin + turn.Duration == Owner.Time)
				{
					queue.RemoveAt(0);
					QueueBegin += turn.Duration;
					turn.Execute();

					return true;
				}
			}

			return false;
		}

		private sealed class CourseTurn : Turn
		{
			public const long FLAT_FACTOR = 5;
			public const long DIAGONAL_FACTOR = 7;

			public static long GetFactor(Course course) => course switch
			{
				Course.Right => FLAT_FACTOR,
				Course.Left => FLAT_FACTOR,
				Course.Top => FLAT_FACTOR,
				Course.Bottom => FLAT_FACTOR,
				Course.RightTop => DIAGONAL_FACTOR,
				Course.RightBottom => DIAGONAL_FACTOR,
				Course.LeftTop => DIAGONAL_FACTOR,
				Course.LeftBottom => DIAGONAL_FACTOR,
				_ => throw EnumNotSupportedException.Create(course),
			};

			public static CourseTurn Create(ScheduledProxy owner, Course course, int count)
			{
				if (count < 1)
				{
					throw new ArgumentOutOfRangeException(nameof(count));
				}

				var factor = GetFactor(course);
				return new CourseTurn(owner, course, factor * count);
			}

			private CourseTurn(ScheduledProxy owner, Course course, long duration) : base(duration)
			{
				Course = course;
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public Course Course { get; }

			public ScheduledProxy Owner { get; }

			public override void Execute()
			{
				if (Owner.Target.Container?.Components[Playground.COLLIDED_NAME] is Collided collided)
				{
					var collidedProxy = (CollidedProxy)Owner.Owner.CreateProxy(collided);
					var offset = Course.ToOffset();
					collidedProxy.Offset(offset);
				}
			}
		}

		private sealed class ProjectileBuilderTurn : Turn
		{
			public ProjectileBuilderTurn(
				IComponentProxy owner,
				Course course,
				long duration,
				int size,
				float damage
			) : base(duration)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Course = course;
				Size = size > 0 ? size : throw new ArgumentOutOfRangeException(nameof(size));
				Damage = damage;
			}

			private IComponentProxy Owner { get; }

			private Course Course { get; }

			private int Size { get; }

			private float Damage { get; }

			public override void Execute()
			{
				var ownerCollided = Owner.GetComponent(Playground.COLLIDED_NAME);
				if (ownerCollided is ICollidedProxy { Current: { } current })
				{
					var playground = Owner.Subject.Playground;

					var subject = playground.CreateSubject();
					playground.Subjects.Add(subject);
					var collided = (ICollidedProxy)subject.AddComponent(Playground.COLLIDED_NAME);

					var bordered = playground.CreateBordered();

					var pivot = GetPivot(current.Target.Border, Course);
					var projectileBorder = CreateRectangle(Course.Invert(), pivot, new Size(Size, Size));
					projectileBorder.Offset(Course.ToOffset());

					bordered.AddCell(projectileBorder);
					collided.Current = bordered;

					var projectile = (ICollisionDamageProxy)subject.AddComponent(Playground.COLLISION_DAMAGE_NAME);
					projectile.Damage = Damage;

					var strength = (IScaleProxy)subject.AddComponent(Playground.STRENGTH_NAME);
					strength.Max = 1;
					strength.Value = 1;

					subject.AddComponent(PlayMaster.COLLISION_HANDLE);

					var scheduler = (IScheduledProxy)subject.AddComponent(Playground.SCHEDULER_NAME);
					for (var i = 0; i < 100; i++)
					{
						scheduler.SetCourse(Course);
					}
				}
			}

			private static Point GetPivot(Rectangle rectangle, Course position) => position switch
			{
				Course.Left => new Point(rectangle.Left, rectangle.Top + rectangle.Height / 2),
				Course.LeftTop => new Point(rectangle.Left, rectangle.Top),
				Course.Top => new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top),
				Course.RightTop => new Point(rectangle.Right, rectangle.Top),
				Course.Right => new Point(rectangle.Right, rectangle.Top + rectangle.Height / 2),
				Course.RightBottom => new Point(rectangle.Right, rectangle.Bottom),
				Course.Bottom => new Point(rectangle.Left + rectangle.Width / 2, rectangle.Bottom),
				Course.LeftBottom => new Point(rectangle.Left, rectangle.Bottom),
				_ => throw EnumNotSupportedException.Create(position),
			};

			private static Rectangle CreateRectangle(Course pivot, Point position, Size size) => pivot switch
			{
				Course.Left => new Rectangle(new Point(position.X, position.Y - size.Width / 2), size),
				Course.LeftTop => new Rectangle(position, size),
				Course.Top => new Rectangle(new Point(position.X - size.Width / 2, position.Y), size),
				Course.RightTop => new Rectangle(new Point(position.X - size.Width, position.Y), size),
				Course.Right => new Rectangle(new Point(position.X - size.Width, position.Y - size.Height / 2), size),
				Course.RightBottom => new Rectangle(new Point(position.X - size.Width, position.Y - size.Height), size),
				Course.Bottom => new Rectangle(new Point(position.X - size.Width / 2, position.Y - size.Height), size),
				Course.LeftBottom => new Rectangle(new Point(position.X, position.Y - size.Height), size),
				_ => throw EnumNotSupportedException.Create(pivot),
			};
		}
	}
}
