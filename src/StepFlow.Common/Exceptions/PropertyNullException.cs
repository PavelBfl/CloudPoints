using System;

namespace StepFlow.Common.Exceptions
{
	public class PropertyNullException : InvalidOperationException
	{
		private const string DEFAULT_MESSAGE = "Required property have null value.";

		public PropertyNullException(string? propertyName)
			: base(DEFAULT_MESSAGE)
		{
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
		}

		public string PropertyName { get; }
	}
}
