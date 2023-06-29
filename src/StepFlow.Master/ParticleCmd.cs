using StepFlow.Core;

namespace StepFlow.Master
{
	internal class ParticleCmd<TSource> : SubjectCmd<TSource>, IParticleCmd<TSource>
		where TSource : Particle
	{
		public ParticleCmd(PlayMaster owner, TSource source) : base(owner, source)
		{
		}
	}
}
