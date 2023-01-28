using System;

namespace StepFlow.Core.Exceptions
{
	public class InvalidCoreException : InvalidOperationException
	{
		private const string ADD_EXISTS_ELEMENT_MESSAGE = "Add to node already exists occupier element.";
		private const string REMOVE_NOT_EXISTS_ELEMENT_MESSAGE = "Remove from node not exists element.";

		internal static InvalidCoreException CreateAddExistsElement() => new InvalidCoreException(ADD_EXISTS_ELEMENT_MESSAGE);

		internal static InvalidCoreException CreateNotExistsElement() => new InvalidCoreException(REMOVE_NOT_EXISTS_ELEMENT_MESSAGE);

		public InvalidCoreException(string? message) : base(message)
		{
		}
	}
}
