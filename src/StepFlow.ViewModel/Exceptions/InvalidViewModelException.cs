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

		internal static InvalidViewModelException CreateInvalidMatchPairs()
			=> new InvalidViewModelException("Ivalid match pairs model and view model.");

		internal static InvalidViewModelException CreateInvalidAccessToSource()
			=> new InvalidViewModelException("Ivalid access to source. Source element is null.");

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

	public class InvalidAccessToMember : InvalidViewModelException
	{
		internal static InvalidAccessToMember CreateInvalidAccessToProperty(string? propertyName)
			=> new InvalidAccessToMember(propertyName, "Invalid access to property.");

		internal static InvalidAccessToMember CreateInvalidInvokeMethod(string? methodName)
			=> new InvalidAccessToMember(methodName, "Invalid invoke method.");

		public InvalidAccessToMember(string? memberName, string message)
			: base(message)
		{
			MemberName = memberName;
		}

		public string? MemberName { get; }
	}
}
