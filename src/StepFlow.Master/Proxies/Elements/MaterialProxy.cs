﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Schedulers;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IMaterialProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : Material
	{
		Scale? Strength { get; }

		Collided Body { get; }

		int Speed { get; set; }

		Action? CurrentAction { get; set; }

		ICollection<SchedulerRunner> Schedulers { get; }

		void OnTick();

		void SetCourse(Course course);

		void Collision(Collided thisCollided, Material otherMaterial, Collided otherCollided);
	}

	internal class MaterialProxy<TTarget> : ProxyBase<TTarget>, IMaterialProxy<TTarget>
		where TTarget : Material
	{
		public MaterialProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public Scale? Strength { get => Target.Strength; protected set => SetValue(x => x.Strength, value); }

		public Collided Body { get => Target.Body; }

		public Action? CurrentAction { get => Target.CurrentAction; set => SetValue(x => x.CurrentAction, value); }

		public ICollection<SchedulerRunner> Schedulers => CreateCollectionProxy(Target.Schedulers);

		public virtual void OnTick()
		{
			if (CurrentAction is { } currentAction)
			{
				if (Owner.TimeAxis.Current == (currentAction.Begin + currentAction.Duration))
				{
					var executorProxy = (ITurnExecutor?)Owner.CreateProxy(currentAction.Executor);
					executorProxy?.Execute();
					CurrentAction = null;
				}
			}

			foreach (var scheduler in Schedulers.Select(Owner.CreateProxy).Cast<ISchedulerRunnerProxy>())
			{
				scheduler.OnTick();
			}
		}

		public virtual void Collision(Collided thisCollided, Material otherMaterial, Collided otherCollided)
		{
			if (Target != otherMaterial && otherCollided.IsRigid)
			{
				((ICollidedProxy)Owner.CreateProxy(Body)).Break();
			}
		}

		public void SetCourse(Course course)
		{
			var factor = course.GetFactor();

			if (Schedulers.Select(x => x.Scheduler).OfType<SchedulerVector>().SelectMany(x => x.Vectors).FirstOrDefault(x => x.Name == "Control") is { } controlVector)
			{
				var offset = course.ToOffset();
				var vector = new Vector2(offset.X, offset.Y) * Speed;
				controlVector.Value = vector;
			}

			// TODO Удалить после доработки планировщиков
			//CurrentAction = new Action()
			//{
			//	Begin = Owner.TimeAxis.Current,
			//	Duration = factor * Speed,
			//	Executor = new SetCourse()
			//	{
			//		Collided = Body,
			//		Course = course,
			//	},
			//};
		}

		public int Speed { get => Target.Speed; set => SetValue(x => x.Speed, value); }
	}
}
