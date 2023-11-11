using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy : ICollidedProxy, IScheduledProxy, IScaleProxy
	{
		
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy
		where TTarget : Subject, ICollided, IScheduled, IScale
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
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
		}

		public void Move()
		{
			if (IsMove)
			{
				Current = Next;
				Break();
			}
		}

		public void Break()
		{
			Next = null;
			IsMove = false;
		}

		ICollided IReadOnlyProxyBase<ICollided>.Target => Target;

		#endregion

		#region IScheduledProxy
		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		public IList<ITurnProxy> Queue => new ListItemsProxy<Turn, IList<Turn>, ITurnProxy>(Owner, Target.Queue);

		public void SetCourse(Course course)
		{
			var factor = course.GetFactor();

			var setCourse = (ISetCourseProxy)Owner.CreateProxy(new SetCourse(Target.Context)
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

			Queue.Add(turnProxy);
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

				return true;
			}
			else
			{
				return false;
			}
		}

		IScheduled IReadOnlyProxyBase<IScheduled>.Target => Target;
		#endregion

		#region IScaleProxy
		public float Value { get => Target.Value; set => SetValue(x => x.Value, value); }

		public float Max { get => Target.Max; set => SetValue(x => x.Max, value); }

		public bool Freeze { get => Target.Freeze; set => SetValue(x => x.Freeze, value); }

		public bool Add(float value)
		{
			var oldValue = Value;
			if (!Freeze)
			{
				var newValue = Value + value;
				if (newValue < 0)
				{
					Value = 0;
				}
				else if (newValue > Max)
				{
					Value = Max;
				}
				else
				{
					Value = newValue;
				}
			}

			return oldValue != Value;
		}

		IScale IReadOnlyProxyBase<IScale>.Target => Target;

		#endregion
	}
}
