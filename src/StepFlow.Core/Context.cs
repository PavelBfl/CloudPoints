using System.Drawing;

namespace StepFlow.Core
{
	public sealed class Context : IContext
	{
		public Intersection.Context IntersectionContext { get; } = new Intersection.Context(new Rectangle(0, 0, 100, 100));
	}
}
