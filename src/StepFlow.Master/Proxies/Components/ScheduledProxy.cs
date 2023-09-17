using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components.Custom;

namespace StepFlow.Master.Proxies.Components
{
	public sealed class ScheduledProxy : ComponentProxy<Scheduled>, IScheduledProxy
	{
		public ScheduledProxy(PlayMaster owner, Scheduled target) : base(owner, target)
		{
		}

		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		private IList<Turn> Queue => new ListProxy<Turn, List<Turn>>(Owner, Target.Queue);

		public ICollection<IComponentProxy> QueueComplete => CreateEvenProxy(Target.QueueComplete);

		public bool IsEmpty => !Queue.Any();

		public void CreateProjectile(Course course)
		{
			var projectileBuilderHandler = (ProjectileBuilderHandler)Subject.AddComponent(Master.Components.Handlers.PROJECTILE_BUILDER);
			projectileBuilderHandler.Disposable = true;
			var projectileSettings = (IProjectileSettingsProxy)Subject.GetComponentRequired(Master.Components.Names.PROJECTILE_SETTINGS);
			projectileSettings.Course = course;

			Add(1, projectileBuilderHandler);
		}

		public void SetCourse(Course course, int stepTime = 1)
		{
			var courseHandler = (CourseHandler)Subject.AddComponent(Master.Components.Handlers.COURSE);
			courseHandler.Disposable = true;
			courseHandler.Course = course;

			Add(CourseHandler.GetFactor(course) * stepTime, courseHandler);
		}

		public void Add(long duration, IHandler? handler) => Add(new Turn(duration, handler?.Target));

		private void Add(Turn turn)
		{
			var queue = Queue;

			if (!queue.Any())
			{
				QueueBegin = Owner.Time;
			}

			queue.Add(turn);
		}

		public bool TryDequeue()
		{
			var result = false;

			while (TrySingleDequeue())
			{
				result = true;
			}

			return result;
		}

		private bool TrySingleDequeue()
		{
			var queue = Queue;

			if (queue.Any())
			{
				var turn = queue[0];
				if (QueueBegin + turn.Duration == Owner.Time)
				{
					queue.RemoveAt(0);
					QueueBegin += turn.Duration;
					((IHandler?)turn.Executor)?.Handle(this);

					if (IsEmpty)
					{
						foreach (var handler in QueueComplete.Cast<IHandler>())
						{
							handler.Handle(this);
						}
					}

					return true;
				}
			}

			return false;
		}
	}
}
