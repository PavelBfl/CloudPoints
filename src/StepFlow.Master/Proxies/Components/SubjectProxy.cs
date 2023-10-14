﻿using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	internal class SubjectProxy : ContainerProxy<Subject>, ISubjectProxy
	{
		public SubjectProxy(PlayMaster owner, Subject target) : base(owner, target)
		{
		}

		public IPlaygroundProxy Playground => Owner.CreateProxy(Target.Owner);
	}
}
