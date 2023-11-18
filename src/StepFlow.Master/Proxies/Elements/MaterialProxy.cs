using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy : ICollidedProxy, IScheduledProxy
	{
		IScaleProxy? Strength { get; }

		void OnTick();
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy
		where TTarget : Material, ICollided, IScheduled
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public IScaleProxy? Strength { get => (IScaleProxy?)Owner.CreateProxy(Target.Strength); protected set => SetValue(x => x.Strength, value?.Target); }

		public virtual void OnTick()
		{
		}

		#region ICollidedProxy
		public IBorderedBaseProxy<IBordered>? Current
		{
			get => (IBorderedBaseProxy<IBordered>?)Owner.CreateProxy(Target.Current);
			set => SetValue(x => x.Current, value?.Target);
		}

		public IBorderedBaseProxy<IBordered>? Next
		{
			get => (IBorderedBaseProxy<IBordered>?)Owner.CreateProxy(Target.Next);
			set => SetValue(x => x.Next, value?.Target);
		}

		public bool IsMove { get => Target.IsMove; set => SetValue(x => x.IsMove, value); }
		public virtual void Collision(ICollidedProxy other)
		{
			((ICollidedProxy)this).Break();
		}

		ICollided IReadOnlyProxyBase<ICollided>.Target => Target;

		#endregion

		#region IScheduledProxy
		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		public IList<ITurnProxy> Queue => new ListItemsProxy<Turn, IList<Turn>, ITurnProxy>(Owner, Target.Queue);

		public void SetCourse(Course course)
		{
			var factor = course.GetFactor();

			var setCourse = (ISetCourseProxy)Owner.CreateProxy(new SetCourse()
			{
				Collided = Target,
				Course = course,
			});

			Enqueue(factor, setCourse);
		}

		public void Enqueue(long duration, ITurnExecutor? turn = null)
		{
			if (duration < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(duration));
			}

			var turnProxy = (ITurnProxy)Owner.CreateProxy(new Turn(duration, turn?.Target));

			var queue = Queue;
			if (!queue.Any())
			{
				QueueBegin = Owner.Time;
			}

			queue.Add(turnProxy);
		}

		public void Dequeue()
		{
			while (TryDequeueSingle())
			{
			}
		}

		private bool TryDequeueSingle()
		{
			if (Queue.FirstOrDefault() is { } first && (QueueBegin + first.Duration) == Owner.Time)
			{
				Queue.RemoveAt(0);
				first.Executor?.Execute();
				QueueBegin = Owner.Time;

				return true;
			}
			else
			{
				return false;
			}
		}

		IScheduled IReadOnlyProxyBase<IScheduled>.Target => Target;
		#endregion
	}
}
