using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	internal class SubjectProxy : ContainerProxy<Subject>, ISubjectProxy
	{
		public SubjectProxy(PlayMaster owner, Subject target) : base(owner, target)
		{
		}

		public string? Name { get => Target.Name; set => SetValue(x => x.Name, value); }

		public IPlaygroundProxy Playground => Owner.CreateProxy(Target.Owner);
	}
}
