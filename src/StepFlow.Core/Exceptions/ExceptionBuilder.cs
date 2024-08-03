using System;

namespace StepFlow.Core.Exceptions
{
	internal static class ExceptionBuilder
	{
		internal static Exception CreatePropertyIsNull(string? propertyName) => new PropertyNullException(propertyName);

		internal static Exception CreateUnknownTypeForCopy(Type originalType) => new InvalidOperationException();

		internal static Exception CreateNonuniqueItemForCopy() => new InvalidOperationException();

		internal static Exception CreateCopiedPropertyIsNull(string propertyName) => new PropertyNullException($"Copied property be null. (Property '{propertyName}')", propertyName);
	}
}
