using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScheduledProxy : IComponentProxy
	{
		long QueueBegin { get; set; }

		void CreateProjectile(Course course);
		void SetCourse(Course course, int stepTime = 1);
	}
}
