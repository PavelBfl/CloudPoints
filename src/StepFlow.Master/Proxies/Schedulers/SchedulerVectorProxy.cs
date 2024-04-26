using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Actions;
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

		public Collided Collided { get => Target.GetCollidedRequired(); set => SetValue(Subject.PropertyRequired(value, nameof(Target.Collided))); }

		public Vector2 CorrectVector { get => Target.CorrectVector; set => SetValue(value); }

		public int IndexCourse { get => Target.IndexCourse; set => SetValue(value); }

		public override void Next()
		{
			var sum = Vector2.Zero;

			foreach (var vector in Target.Vectors)
			{
				sum += vector.Value;
			}

			if (sum != CorrectVector)
			{
				CorrectVector = sum;
				IndexCourse = 0;
			}

			if (CourseExtensions.GetCourseStep(sum, IndexCourse) is { } course)
			{
				var length = (int)sum.Length();
				if (length > 0)
				{
					Current = new Turn(
						100 / length,
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
			else
			{
				Current = new Turn(1, null);
			}
		}
	}
}
