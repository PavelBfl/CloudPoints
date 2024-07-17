namespace StepFlow.Core
{
	public sealed class Context : IContext
	{
		public Intersection.Context IntersectionContext { get; } = new Intersection.Context();
	}
}
