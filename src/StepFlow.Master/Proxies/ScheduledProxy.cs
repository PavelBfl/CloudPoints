using System;
using System.Collections.Generic;
using System.Linq;
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

		public void SetCourse(Course course)
		{
			var queueProxy = (ListProxy<Turn, List<Turn>>)Owner.CreateProxy(Target.Queue);

			if (!queueProxy.Any())
			{
				QueueBegin = Owner.Time;
			}

			queueProxy.Add(CourseTurn.Create(this, course, 1));
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
	}
}
