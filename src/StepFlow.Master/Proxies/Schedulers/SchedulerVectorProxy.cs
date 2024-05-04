using System;
using System.Collections.Generic;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
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

		public float CorrectFactor { get => Target.CorrectFactor; set => SetValue(value); }

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

			OffsetAndClearVectors();

			var factor = sum.X / sum.Y;
			if (MathF.Abs(factor - CorrectFactor) > 0.00001f)
			{
				CorrectFactor = factor;
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

		private void OffsetAndClearVectors()
		{
			var removedVectors = new List<CourseVector>();
			foreach (var vector in Target.Vectors)
			{
				switch (vector.Operation)
				{
					case DeltaOperation.Sum:
						SetValue(vector, vector.Value + vector.Delta);
						break;
					case DeltaOperation.Mul:
						SetValue(vector, vector.Value * vector.Delta);
						if (vector.Value.LengthSquared() < 1)
						{
							removedVectors.Add(vector);
						}
						break;
					default: throw EnumNotSupportedException.Create(vector.Operation);
				}
			}

			if (removedVectors.Count > 0)
			{
				var vectors = Owner.CreateCollectionProxy(Target.Vectors);
				foreach (var vector in removedVectors)
				{
					vectors.Remove(vector);
				}
			}
		}

		private void SetValue(CourseVector courseVector, Vector2 newValue)
		{
			if (courseVector.Value != newValue)
			{
				var courseVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(courseVector);
				courseVectorProxy.Value = newValue;
			}
		}
	}
}
