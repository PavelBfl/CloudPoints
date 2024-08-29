using System;

namespace StepFlow.Intersection
{
	internal static class ExceptionBuilder
	{
		internal static Exception CreateUnexpectedEnable()
			=> new InvalidOperationException("Use shape with unexpected enable.");

		internal static Exception CreateUnknownContext()
			=> new InvalidOperationException("Use shape with unknown context.");
	}
}
