using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public class SubjectProxy : ContainerProxy<Subject>, ISubjectProxy
	{
		public SubjectProxy(PlayMaster owner, Subject target) : base(owner, target)
		{
		}

		public IPlaygroundProxy Playground => (IPlaygroundProxy)Owner.CreateProxy(Target.Owner);
	}
}
