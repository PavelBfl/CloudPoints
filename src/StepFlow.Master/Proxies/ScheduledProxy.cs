using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class ScheduledProxy : ProxyBase<Scheduled>
	{
		public ScheduledProxy(PlayMaster owner, Scheduled target) : base(owner, target)
		{
		}

		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		public void CreateProjectile(Course course) => Add(new ProjectileBuilderTurn(this, course, 1, 10, 10));

		public void SetCourse(Course course) => Add(CourseTurn.Create(this, course, 1));

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
					var subject = playgroundProxy.CreateSubject();
					playgroundProxy.Subjects.Add(subject);
					var subjectProxy = (SubjectProxy<Subject>)Owner.Owner.CreateProxy(subject);

					subjectProxy.AddComponent(Playground.COLLIDED_NAME);
					var collided = (Collided)subjectProxy.GetComponent(Playground.COLLIDED_NAME);
					var collidedProxy = (CollidedProxy)Owner.Owner.CreateProxy(collided);

					var bordered = playgroundProxy.CreateBordered();
					var borderedProxy = (BorderedProxy)Owner.Owner.CreateProxy(bordered);

					var border = current.Border;
					var center = new System.Drawing.Point(border.X + border.Width / 2, border.Y + border.Height / 2);
					borderedProxy.AddCell(new System.Drawing.Rectangle(
						center.X - Size / 2,
						center.Y - Size / 2,
						Size,
						Size
					));
					collidedProxy.Current = borderedProxy.Target;

					subjectProxy.AddComponent(Playground.PROJECTILE_NAME);
					var projectile = subjectProxy.GetComponent(Playground.PROJECTILE_NAME);
					var projectileProxy = (ProjectileProxy)Owner.Owner.CreateProxy(projectile);
					projectileProxy.Damage = Damage;

					subjectProxy.AddComponent(Playground.SCHEDULER_NAME);
					var scheduler = subjectProxy.GetComponent(Playground.SCHEDULER_NAME);
					var schedulerProxy = (ScheduledProxy)Owner.Owner.CreateProxy(scheduler);
					for (var i = 0; i < 20; i++)
					{
						schedulerProxy.SetCourse(Course);
					}
				}
			}
		}
	}
}
