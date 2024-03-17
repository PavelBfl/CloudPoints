using System;

namespace StepFlow.Core.Exceptions
{
	public class PropertyNullException : InvalidOperationException
	{
		public const string MESSAGE_FORMAT = "Property cannot be null. (Property '{0}')";

		public PropertyNullException(string? propertyName)
			: base(string.Format(MESSAGE_FORMAT, propertyName))
		{
			PropertyName = propertyName;
		}

		public string? PropertyName { get; }
	}
}
