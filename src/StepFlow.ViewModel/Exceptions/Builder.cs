using System;

namespace StepFlow.ViewModel.Exceptions
{
	internal static class Builder
	{
		public static InvalidOperationException CreateInvalidSync()
			=> new InvalidOperationException("Failed sync command with piece.");

		public static InvalidOperationException CreateRemoveMark()
			=> new InvalidOperationException("Registry counter is zero.");

		public static InvalidOperationException CreateInvalidMatchPairs()
			=> new InvalidOperationException("Invalid match pairs model and view model.");

		public static InvalidOperationException CreateInvalidAccessToSource()
			=> new InvalidOperationException("Invalid access to source. Source element is null.");

		public static InvalidCastException CreateUnknownCommand()
			=> new InvalidCastException("Unknown command model.");

		public static InvalidOperationException CreateUnknownModel()
			=> new InvalidOperationException("Unknown model.");
	}
}
