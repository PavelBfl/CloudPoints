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
		new Subject? Target { get; set; }

		int Begin { get; set; }

		void OnTick();
	}

	internal abstract class SchedulerProxy<TScheduler> : ElementBaseProxy<TScheduler>, ISchedulerProxy<TScheduler>
		where TScheduler : Scheduler
	{
		protected SchedulerProxy(PlayMaster owner, TScheduler target) : base(owner, target)
		{
		}

		protected TScheduler GetBaseTarget() => ((IProxyBase<TScheduler>)this).Target;

		public new Subject? Target { get => base.Target.Target; set => SetValue(x => x.Target, value); }

		public int Begin { get => base.Target.Begin; set => SetValue(x => x.Begin, value); }

		public Turn? Current { get => base.Target.Current; set => SetValue(x => x.Current, value); }

		public void OnTick()
		{
			while (SingleTick()) ;
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

		protected abstract void Next();
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

		public int CurrentIndex { get => GetBaseTarget().CurrentIndex; set => SetValue(x => x.CurrentIndex, value); }

		public IList<Course> Path => CreateListProxy(GetBaseTarget().Path);

		public bool IsLast { get => GetBaseTarget().IsLast; set => SetValue(x => x.IsLast, value); }

		public Turn? Last { get => GetBaseTarget().Last; set => SetValue(x => x.Last, value); }

		protected override void Next()
		{
			if (0 <= CurrentIndex && CurrentIndex < Path.Count)
			{
				var material = (Material)Target;
				var course = Path[CurrentIndex];
				Current = new Turn(
					course.GetFactor() * material.Speed,
					new SetCourse()
					{
						Collided = material.Body,
						Course = course,
					}
				);
				CurrentIndex++;

				if (CurrentIndex >= Path.Count)
				{
					IsLast = true;
				}
			}
			else if (IsLast)
			{
				Current = Last;
				IsLast = false;
			}
			else
			{
				Current = null;
				IsLast = false;
			}
		}
	}

	public interface ISchedulerVectorProxy : ISchedulerProxy<SchedulerVector>
	{
		
	}

	internal sealed class SchedulerVectorProxy : SchedulerProxy<SchedulerVector>, ISchedulerVectorProxy
	{
		public SchedulerVectorProxy(PlayMaster owner, SchedulerVector target) : base(owner, target)
		{
		}

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

		protected override void Next()
		{
			var sum = PointF.Empty;
			var material = ((Material)Target);
			var center = material.Body.Current.Bounds.GetCenter();

			foreach (var endPoint in GetBaseTarget().EndPoints)
			{
				var vector = GetVector(center, endPoint);
				sum.X += vector.X;
				sum.Y += vector.Y;
			}

			var duration = GetDuration(sum);
			var course = GetCourse(sum);

			Current = new Turn(
				duration,
				new SetCourse()
				{
					Collided = material.Body,
					Course = course,
				}
			);
		}

		public static int GetDuration(PointF vector)
		{
			var length = GetLength(vector);

			var result = 100 / length;

			return (int)result;
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
				return Course.LeftBottom;
			}
			else if (step * -5 < angle && angle <= step * -3)
			{
				return Course.Bottom;
			}
			else if (step * -3 < angle && angle <= step * -1)
			{
				return Course.RightBottom;
			}
			else if (step * -1 < angle && angle <= step * 1)
			{
				return Course.Right;
			}
			else if (step * 1 < angle && angle <= step * 3)
			{
				return Course.RightTop;
			}
			else if (step * 3 < angle && angle <= step * 5)
			{
				return Course.Top;
			}
			else if (step * 5 < angle && angle <= step * 7)
			{
				return Course.LeftTop;
			}
			else
			{
				return Course.Left;
			}
		}
	}
}
