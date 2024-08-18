using System;
using StepFlow.Common.Exceptions;

namespace StepFlow.Core.Exceptions
{
	internal static class ExceptionBuilder
	{
		internal static Exception CreateUnknownTypeForCopy(Type originalType) => new InvalidOperationException();

		internal static Exception CreateNonuniqueItemForCopy() => new InvalidOperationException();

		internal static Exception CreateCopiedPropertyIsNull(string propertyName) => new PropertyNullException(propertyName);

		internal static Exception CreateUnknownIntersectionContext() => new InvalidOperationException("Unknown intersection context for play ground context.");
	}
}
