using System;

namespace StepFlow.Intersection
{
	internal static class ExceptionBuilder
	{
		internal static Exception CreateShapeAlreadyContext()
			=> new InvalidOperationException("Shape already context");
	}
}
