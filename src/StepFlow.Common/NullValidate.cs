using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using StepFlow.Common.Exceptions;

namespace StepFlow.Common
{
	public static class NullValidate
	{
		[return: NotNull]
		public static T PropertyRequired<T>([NotNull] T propertyValue, [CallerMemberName] string? propertyName = null)
			=> propertyValue is { } ? propertyValue : throw new PropertyNullException(propertyName);

		public static T PropertyRequired<T>([NotNull] T? propertyValue, [CallerMemberName] string? propertyName = null)
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
