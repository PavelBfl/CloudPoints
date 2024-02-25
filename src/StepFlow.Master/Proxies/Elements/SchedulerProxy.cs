using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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
		Turn? Current { get; set; }

		void Next();
	}

	internal abstract class SchedulerProxy<TScheduler> : ElementBaseProxy<TScheduler>, ISchedulerProxy<TScheduler>
		where TScheduler : Scheduler
	{
		protected SchedulerProxy(PlayMaster owner, TScheduler target) : base(owner, target)
		{
		}

		public Turn? Current { get => Target.Current; set => SetValue(x => x.Current, value); }

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

		public override void Next()
		{
			if (Range.Value < Range.Max)
			{
				var schedulerProxy = (ISchedulerProxy<Scheduler>)Owner.CreateProxy(Source);
				schedulerProxy.Next();
				Current = schedulerProxy.Current;

				if (Current is { } current)
				{
					var rangeProxy = (IScaleProxy)Owner.CreateProxy(Range);
					rangeProxy.Add((int)current.Duration);
				}
			}
			else
			{
				Current = null;
			}
		}
	}

	public interface ISchedulerCollectionProxy : ISchedulerProxy<SchedulerCollection>
	{
		int Index { get; set; }
	}

	internal sealed class SchedulerCollectionProxy : SchedulerProxy<SchedulerCollection>, ISchedulerCollectionProxy
	{
		public SchedulerCollectionProxy(PlayMaster owner, SchedulerCollection target) : base(owner, target)
		{
		}

		public int Index { get => Target.Index; set => SetValue(x => x.Index, value); }

		public override void Next()
		{
			if (0 <= Index && Index < Target.Turns.Count)
			{
				Current = Target.Turns[Index];
				Index++;
			}
			else
			{
				Current = null;
			}
		}
	}

	public interface ISchedulerUnionProxy : ISchedulerProxy<SchedulerCollection>
	{
		int Index { get; set; }
	}

	internal sealed class SchedulerUnionProxy : SchedulerProxy<SchedulerUnion>
	{
		public SchedulerUnionProxy(PlayMaster owner, SchedulerUnion target) : base(owner, target)
		{
		}

		public int Index { get => Target.Index; set => SetValue(x => x.Index, value); }

		public override void Next()
		{
			while (true)
			{
				if (TryNext(out var result))
				{
					Current = result;
					return;
				}
			}
		}

		private bool TryNext(out Turn? result)
		{
			if (0 <= Index && Index < Target.Schedulers.Count)
			{
				var currentScheduler = Target.Schedulers[Index];
				var currentSchedulerProxy = (ISchedulerProxy<Scheduler>)Owner.CreateProxy(currentScheduler);

				currentSchedulerProxy.Next();
				if (currentSchedulerProxy.Current is { } currentTurn)
				{
					result = currentTurn;
					return true;
				}
				else
				{
					Index++;
					result = null;
					return false;
				}
			}
			else
			{
				result = null;
				return true;
			}
		}
	}

	public interface ISchedulerRunnerProxy : IProxyBase<SchedulerRunner>
	{
		int Begin { get; set; }

		Turn? Current { get; set; }

		Scheduler? Scheduler { get; set; }

		void OnTick();
	}

	internal sealed class SchedulerRunnerProxy : ProxyBase<SchedulerRunner>, ISchedulerRunnerProxy
	{
		public SchedulerRunnerProxy(PlayMaster owner, SchedulerRunner target) : base(owner, target)
		{
		}

		public int Begin { get => Target.Begin; set => SetValue(x => x.Begin, value); }

		public Turn? Current { get => Target.Current; set => SetValue(x => x.Current, value); }

		public Scheduler? Scheduler { get => Target.Scheduler; set => SetValue(x => x.Scheduler, value); }

		public void OnTick()
		{
			while (SingleTick()) ;
		}

		private bool SingleTick()
		{
			if (Current is null)
			{
				var schedulerProxy = (ISchedulerProxy<Scheduler>?)Owner.CreateProxy(Scheduler);
				schedulerProxy?.Next();
				Current = schedulerProxy?.Current;
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
	}
}
