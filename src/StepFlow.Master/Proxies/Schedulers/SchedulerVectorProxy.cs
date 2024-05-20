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

		public int IndexCourse { get => Target.IndexCourse; set => SetValue(value); }

		public override void Next()
		{
			if (Target.Vectors.Count == 0)
			{
				Current = null;
				return;
			}

			var sum = Vector2.Zero;

			foreach (var vector in Target.Vectors)
			{
				sum += vector.Value;
			}

			if (sum != Vector2.Zero)
			{
				Current = new Turn(
					1,
					new SetCourse()
					{
						Collided = Collided,
						Course = sum
					}
				);
			}
			else
			{
				Current = new Turn(1, null);
			}

			OffsetAndClearVectors(Current.Value.Duration);
		}

		private void OffsetAndClearVectors(int duration)
		{
			foreach (var vector in Target.Vectors)
			{
				var deltaPower = Matrix3x2.Identity;
				for (var i = 0; i < duration; i++)
				{
					deltaPower *= vector.Delta;
				}

				var newValue = Vector2.Transform(vector.Value, deltaPower);
				if (vector.Value != newValue)
				{
					var courseVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(vector);
					courseVectorProxy.Value = newValue;
				}
			}
		}
	}
}
