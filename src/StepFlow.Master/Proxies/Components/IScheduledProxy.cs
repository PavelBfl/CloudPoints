using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScheduledProxy : IProxyBase<IScheduled>
	{
		long QueueBegin { get; set; }

		IList<ITurnProxy> Queue { get; }

		void Dequeue();

		void SetCourse(Course course);
	}
}
