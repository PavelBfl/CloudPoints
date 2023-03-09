using System;

namespace StepFlow.Core.Exceptions
{
	public class CoreException : Exception
	{
		public CoreException(string? message) : base(message)
		{
		}
	}
}
