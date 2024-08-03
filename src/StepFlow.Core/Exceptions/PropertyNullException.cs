using System;

namespace StepFlow.Core.Exceptions
{
	public class PropertyNullException : InvalidOperationException
	{
		public const string MESSAGE_FORMAT = "Property cannot be null. (Property '{0}')";

		public PropertyNullException(string? propertyName)
			: this(string.Format(MESSAGE_FORMAT, propertyName), propertyName)
		{
		}

		public PropertyNullException(string message, string? propertyName)
			: base(message)
		{
			PropertyName = message;
		}

		public string? PropertyName { get; }
	}
}
