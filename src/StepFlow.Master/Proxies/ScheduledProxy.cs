using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class ScheduledProxy : ComponentProxy<Scheduled>
	{
		public ScheduledProxy(PlayMaster owner, Scheduled target) : base(owner, target)
		{
		}

		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		public void CreateProjectile(Course course) => Add(new ProjectileBuilderTurn(this, course, 1, 10, 10));

		public void SetCourse(Course course, int stepTime = 1) => Add(CourseTurn.Create(this, course, stepTime));

		private void Add(Turn turn)
		{
			var queueProxy = (ListProxy<Turn, List<Turn>>)Owner.CreateProxy(Target.Queue);

			if (!queueProxy.Any())
			{
				QueueBegin = Owner.Time;
			}

			queueProxy.Add(turn);
		}

		public bool TryDequeue()
		{
			var queueProxy = (ListProxy<Turn, List<Turn>>)Owner.CreateProxy(Target.Queue);

			if (queueProxy.Any())
			{
				var turn = queueProxy[0];
				if (QueueBegin + turn.Duration == Owner.Time)
				{
					queueProxy.RemoveAt(0);
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
				ScheduledProxy owner,
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

			private ScheduledProxy Owner { get; }

			private Course Course { get; }

			private int Size { get; }

			private float Damage { get; }

			public override void Execute()
			{
				var ownerCollided = Owner.Target.Container.Components[Playground.COLLIDED_NAME];
				if (ownerCollided is Collided { Current: { } current })
				{
					var playgroundProxy = (PlaygroundProxy)Owner.Owner.CreateProxy(Owner.Owner.Playground);

					var subject = playgroundProxy.CreateSubjectProxy();
					var collided = subject.AddComponentProxy<CollidedProxy>(Playground.COLLIDED_NAME);

					var bordered = playgroundProxy.CreateBordered();
					var borderedProxy = (BorderedProxy)Owner.Owner.CreateProxy(bordered);

					var pivot = GetPivot(current.Border, Course);
					var projectileBorder = CreateRectangle(Course.Invert(), pivot, new Size(Size, Size));
					projectileBorder.Offset(Course.ToOffset());

					borderedProxy.AddCell(projectileBorder);
					collided.Current = borderedProxy.Target;

					var projectile = subject.AddComponentProxy<ProjectileProxy>(Playground.PROJECTILE_NAME);
					projectile.Damage = Damage;

					var scheduler = subject.AddComponentProxy<ScheduledProxy>(Playground.SCHEDULER_NAME);
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
