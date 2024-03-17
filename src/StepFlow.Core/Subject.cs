using System.Runtime.CompilerServices;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Subject
	{
		public static T PropertyRequired<T>(T value, [CallerMemberName] string? propertyName = null)
			=> value ?? throw ExceptionBuilder.CreatePropertyIsNull(propertyName);

		public Subject()
		{
		}

		public string? Name { get; set; }
	}
}
