using System;
using System.Drawing;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerVectorProxy : ISchedulerProxy<SchedulerVector>
	{

	}

	internal sealed class SchedulerVectorProxy : SchedulerProxy<SchedulerVector>, ISchedulerVectorProxy
	{
		public SchedulerVectorProxy(PlayMaster owner, SchedulerVector target) : base(owner, target)
		{
		}

		public Collided Collided { get => Target.Collided; set => SetValue(x => x.Collided, value); }

		public Vector2 CorrectVector { get => Target.CorrectVector; set => SetValue(x => x.CorrectVector, value); }

		public int IndexCourse { get => Target.IndexCourse; set => SetValue(x => x.IndexCourse, value); }

		public override void Next()
		{
			var sum = Vector2.Zero;

			foreach (var vector in Target.Vectors)
			{
				sum += vector;
			}

			if (sum != CorrectVector)
			{
				CorrectVector = sum;
				IndexCourse = 0;
			}


			if (CourseExtensions.GetCourseStep(sum, IndexCourse) is { } course)
			{
				var length = (int)sum.Length();
				Current = new Turn(
					100 - length,
					new SetCourse()
					{
						Collided = Collided,
						Course = course
					}
				);

				IndexCourse++;
			}
			else
			{
				Current = new Turn(1, null);
			}
		}

		private static Course GetCourse(PointF vector)
		{
			var angle = MathF.Atan2(vector.Y, vector.X);

			const double step = Math.PI / 8;
			if (-Math.PI < angle && angle <= step * -7)
			{
				return Course.Left;
			}
			else if (step * -7 < angle && angle <= step * -5)
			{
				return Course.LeftTop;
			}
			else if (step * -5 < angle && angle <= step * -3)
			{
				return Course.Top;
			}
			else if (step * -3 < angle && angle <= step * -1)
			{
				return Course.RightTop;
			}
			else if (step * -1 < angle && angle <= step * 1)
			{
				return Course.Right;
			}
			else if (step * 1 < angle && angle <= step * 3)
			{
				return Course.RightBottom;
			}
			else if (step * 3 < angle && angle <= step * 5)
			{
				return Course.Bottom;
			}
			else if (step * 5 < angle && angle <= step * 7)
			{
				return Course.LeftBottom;
			}
			else
			{
				return Course.Left;
			}
		}
	}
}
