using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
    public interface IScheduledProxy : IComponentProxy
	{
		long QueueBegin { get; set; }
		bool IsEmpty { get; }

		void Add(long duration, IHandlerProxy? handler);
		void CreateProjectile(Course course);
		void SetCourse(Course course, int stepTime = 1);
		bool TryDequeue();
	}
}
