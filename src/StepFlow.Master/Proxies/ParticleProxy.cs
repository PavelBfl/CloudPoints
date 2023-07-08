using MoonSharp.Interpreter;
using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public class ParticleProxy<TTarget> : SubjectProxy<TTarget>
		where TTarget : Particle
	{
		[MoonSharpHidden]
		public ParticleProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}
	}
}
