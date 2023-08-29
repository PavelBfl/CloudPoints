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

		public long Time { get => Target.Time; private set => SetValue(x => x.Time, value); }

		public Course? Course { get => Target.Course; private set => SetValue(x => x.Course, value); }

		public void Clear()
		{
			Time = 0;
			Course = null;
		}

		public void SetCourse(Course course)
		{
			var timeOffset = course switch
			{
				Core.Course.Right => 5,
				Core.Course.Left => 5,
				Core.Course.Top => 5,
				Core.Course.Bottom => 5,
				Core.Course.RightTop => 7,
				Core.Course.RightBottom => 7,
				Core.Course.LeftTop => 7,
				Core.Course.LeftBottom => 7,
				_ => throw EnumNotSupportedException.Create(course),
			};

			Time = Owner.Time + timeOffset;
			Course = course;
		}
	}
}
