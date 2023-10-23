using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	internal sealed class ScheduledProxy : ComponentProxy<Scheduled>, IScheduledProxy
	{
		public ScheduledProxy(PlayMaster owner, Scheduled target) : base(owner, target)
		{
		}

		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		private IList<Turn> Queue => new ListProxy<Turn, List<Turn>>(Owner, Target.Queue);

		public ICollection<IHandlerProxy> QueueComplete => CreateEvenProxy(Target.QueueComplete);

		public bool IsEmpty => !Queue.Any();

		public void CreateProjectile(Course course)
		{
			var projectileBuilderHandler = Subject.AddHandler(nameof(Handlers.ProjectileBuilder), true);
			var projectileSettings = (IProjectileSettingsProxy)Subject.GetComponentRequired(Master.Components.Names.PROJECTILE_SETTINGS);
			projectileSettings.Course = course;

			Add(1, projectileBuilderHandler);
		}

		public void SetCourse(Course course, int stepTime = 1)
		{
			var subject = Owner.GetPlaygroundProxy().CreateSubject();
			var setCourseHandler = subject.AddHandler(nameof(Handlers.SetCourseHandler), true);
			var courseHandler = (ISetCourseProxy)subject.AddComponent(Master.Components.Types.SET_COURSE);
			courseHandler.Course = course;

			Add(course.GetFactor() * stepTime, setCourseHandler);
		}

		public void Add(long duration, IHandlerProxy? handler) => Add(new Turn(duration, (Handler?)handler?.Target));

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
					((IHandlerProxy?)Owner.CreateProxy(turn.Executor))?.Handle(this);

					if (IsEmpty)
					{
						foreach (var handler in QueueComplete.Cast<IHandlerProxy>())
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
