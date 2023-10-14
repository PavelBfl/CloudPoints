using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	internal sealed class SetCourseProxy : ComponentProxy<SetCourse>, ISetCourseProxy
	{
		public SetCourseProxy(PlayMaster owner, SetCourse target) : base(owner, target)
		{
		}

		public Course Course { get => Target.Course; set => SetValue(x => x.Course, value); }
	}
}
