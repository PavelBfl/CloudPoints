using System;
using System.Runtime.CompilerServices;
using StepFlow.Core.Exceptions;
using StepFlow.Domains;

namespace StepFlow.Core
{
	public class Subject
	{
		public static T PropertyRequired<T>(T? value, [CallerMemberName] string? propertyName = null)
			where T : class
			=> value ?? throw ExceptionBuilder.CreatePropertyIsNull(propertyName);

		public static T PropertyRequired<T>(T? value, [CallerMemberName] string? propertyName = null)
			where T : struct
			=> value ?? throw ExceptionBuilder.CreatePropertyIsNull(propertyName);

		protected static void ThrowIfOriginalNull(object? original)
		{
			if (original is null)
			{
				throw new ArgumentNullException(nameof(original));
			}
		}

		public Subject()
		{
		}

		public Subject(SubjectDto original)
		{
			ThrowIfOriginalNull(original);

			Name = original.Name;
		}


		public string? Name { get; set; }
	}
}
