using System;

namespace StepFlow.Core.Exceptions
{
	internal static class ExceptionBuilder
	{
		internal static Exception CreatePropertyIsNull(string? propertyName) => new PropertyNullException(propertyName);
	}
}
