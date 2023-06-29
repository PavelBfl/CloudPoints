using StepFlow.Core;

namespace StepFlow.Master
{
	internal class SubjectCmd<TSource> : ContainerCmd<TSource>, ISubjectCmd<TSource>
		where TSource : Subject
	{
		public SubjectCmd(PlayMaster owner, TSource source) : base(owner, source)
		{
		}
	}
}
