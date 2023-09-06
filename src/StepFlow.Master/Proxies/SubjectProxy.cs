using MoonSharp.Interpreter;
using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public class SubjectProxy<TTarget> : ContainerProxy<TTarget>
		where TTarget : Subject
	{
		[MoonSharpHidden]
		public SubjectProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		[MoonSharpHidden]
		public PlaygroundProxy GetPlaygroundProxy() => (PlaygroundProxy)Owner.CreateProxy(Target.Owner);
	}
}
