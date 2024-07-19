using System;
using System.Diagnostics.CodeAnalysis;
using StepFlow.Common.Exceptions;

namespace StepFlow.Common
{
	public static class NullValidateExtensions
	{
		[return: NotNull]
		public static T PropertyRequired<T>([NotNull] this T propertyValue, string propertyName)
			=> propertyValue is { } ? propertyValue : throw new PropertyNullException(propertyName);

		public static T PropertyRequired<T>([NotNull] this T? propertyValue, string propertyName)
			where T : struct
			=> propertyValue is { } ? propertyValue.Value : throw new PropertyNullException(propertyName);

		public static void ThrowIfArgumentNull(object? value, string? name)
		{
			if (value is null)
			{
				throw new ArgumentNullException(name);
			}
		}
	}
}
