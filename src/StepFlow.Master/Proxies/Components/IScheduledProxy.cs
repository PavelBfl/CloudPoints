using StepFlow.Core;
using StepFlow.Master.Proxies.Components.Custom;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScheduledProxy : IComponentProxy
	{
		long QueueBegin { get; set; }
		bool IsEmpty { get; }

		void Add(long duration, IHandler? handler);
		void CreateProjectile(Course course);
		void SetCourse(Course course, int stepTime = 1);
		bool TryDequeue();
	}
}
