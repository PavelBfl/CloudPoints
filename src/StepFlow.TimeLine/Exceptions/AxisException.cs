using System;

namespace StepFlow.TimeLine.Exceptions
{
	public class AxisException : Exception
	{
		public AxisException(string? message, Exception? innerException = null)
			: base(message, innerException)
		{
		}
	}
}
