using System;
using System.Runtime.Serialization;

namespace StepFlow.ViewModel.Exceptions
{
	public class InvalidViewModelException : InvalidOperationException
	{
		private const string INVALID_SYNC_MESSAGE = "Failed sync command with piece.";
		private const string INVALID_UNREGICSTRY_MESSAGE = "Registry counter is zero.";

		internal static InvalidViewModelException CreateInvalidSync()
			=> new InvalidViewModelException(INVALID_SYNC_MESSAGE);

		internal static InvalidViewModelException CreateUnregistryMark()
			=> new InvalidViewModelException(INVALID_UNREGICSTRY_MESSAGE);

		public InvalidViewModelException()
		{
		}

		public InvalidViewModelException(string message) : base(message)
		{
		}

		public InvalidViewModelException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidViewModelException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
