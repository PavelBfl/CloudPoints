using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
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

		private SchedulerPath GetBaseTarget() => ((IProxyBase<SchedulerPath>)this).Target;

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
}
