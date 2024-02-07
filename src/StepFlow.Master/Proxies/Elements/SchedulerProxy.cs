using System;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface ISchedulerProxy<out TScheduler> : IElementBaseProxy<TScheduler>
		where TScheduler : Scheduler
	{
		int Begin { get; set; }

		Turn? Current { get; set; }

		void OnTick();

		void Next();
	}

	internal abstract class SchedulerProxy<TScheduler> : ElementBaseProxy<TScheduler>, ISchedulerProxy<TScheduler>
		where TScheduler : Scheduler
	{
		protected SchedulerProxy(PlayMaster owner, TScheduler target) : base(owner, target)
		{
		}

		public int Begin { get => Target.Begin; set => SetValue(x => x.Begin, value); }

		public Turn? Current { get => Target.Current; set => SetValue(x => x.Current, value); }

		public void OnTick()
		{
			OnTickInner();
			while (SingleTick()) ;
		}

		protected virtual void OnTickInner()
		{
		}

		private bool SingleTick()
		{
			if (Current is null)
			{
				Next();
			}

			if (Current is { } current)
			{
				if (Owner.TimeAxis.Count == (Begin + current.Duration))
				{
					var executor = (ITurnExecutor?)Owner.CreateProxy(current.Executor);
					executor?.Execute();

					Begin += (int)current.Duration;
					Current = null;
					return true;
				}
			}

			return false;
		}

		public abstract void Next();
	}

	public interface ISchedulerPathProxy : ISchedulerProxy<SchedulerPath>
	{
		int CurrentIndex { get; set; }

		IList<Course> Path { get; }
	}

	internal sealed class SchedulerPathProxy : SchedulerProxy<SchedulerPath>, ISchedulerPathProxy
	{
		public SchedulerPathProxy(PlayMaster owner, SchedulerPath target) : base(owner, target)
		{
		}

		public int CurrentIndex { get => Target.CurrentIndex; set => SetValue(x => x.CurrentIndex, value); }

		public IList<Course> Path => CreateListProxy(Target.Path);

		public bool IsLast { get => Target.IsLast; set => SetValue(x => x.IsLast, value); }

		public Turn? Last { get => Target.Last; set => SetValue(x => x.Last, value); }

		public override void Next()
		{
			//if (0 <= CurrentIndex && CurrentIndex < Path.Count)
			//{
			//	var material = (Material)Target;
			//	var course = Path[CurrentIndex];
			//	Current = new Turn(
			//		course.GetFactor() * material.Speed,
			//		new SetCourse()
			//		{
			//			Collided = material.Body,
			//			Course = course,
			//		}
			//	);
			//	CurrentIndex++;

			//	if (CurrentIndex >= Path.Count)
			//	{
			//		IsLast = true;
			//	}
			//}
			//else if (IsLast)
			//{
			//	Current = Last;
			//	IsLast = false;
			//}
			//else
			//{
			//	Current = null;
			//	IsLast = false;
			//}
		}
	}

	public interface ISchedulerVectorProxy : ISchedulerProxy<SchedulerVector>
	{
		
	}

	internal sealed class SchedulerVectorProxy : SchedulerProxy<SchedulerVector>, ISchedulerVectorProxy
	{

		private static float GetLength(PointF vector) => MathF.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y));

		private static PointF GetVector(Point beginPoint, EndPoint endPoint)
		{
			var result = new PointF(
				endPoint.Point.X - beginPoint.X,
				endPoint.Point.Y - beginPoint.Y
			);

			var length = GetLength(result);
			var factor = endPoint.Force / length;

			result.X *= factor;
			result.Y *= factor;

			return result;
		}

		public SchedulerVectorProxy(PlayMaster owner, SchedulerVector target) : base(owner, target)
		{
		}

		public Collided Collided { get => Target.Collided; set => SetValue(x => x.Collided, value); }

		public override void Next()
		{
			var sum = PointF.Empty;
			var center = Collided.Current.Bounds.GetCenter();

			foreach (var endPoint in Target.EndPoints)
			{
				var vector = GetVector(center, endPoint);
				sum.X += vector.X;
				sum.Y += vector.Y;
			}

			var length = (int)GetLength(sum);

			if (length > 0)
			{
				Current = new Turn(
					100 - length,
					new SetCourse()
					{
						Collided = Collided,
						Course = GetCourse(sum),
					}
				);
			}
		}

		public static Course GetCourse(PointF vector)
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

	public interface ISchedulerLimitProxy : ISchedulerProxy<SchedulerLimit>
	{
	
	}

	internal sealed class SchedulerLimitProxy : SchedulerProxy<SchedulerLimit>, ISchedulerLimitProxy
	{
		public SchedulerLimitProxy(PlayMaster owner, SchedulerLimit target) : base(owner, target)
		{
		}

		public Scheduler? Source { get => Target.Source; set => SetValue(x => x.Source, value); }

		public Scale? Range { get => Target.Range; set => SetValue(x => x.Range, value); }

		protected override void OnTickInner()
		{
			base.OnTickInner();

			var rangeProxy = (IScaleProxy)Owner.CreateProxy(Range);
			rangeProxy.Increment();
		}

		public override void Next()
		{
			if (Range.Value < Range.Max)
			{
				var schedulerProxy = (ISchedulerProxy<Scheduler>)Owner.CreateProxy(Source);
				schedulerProxy.Next();
				Current = schedulerProxy.Current;
			}
		}
	}
}
