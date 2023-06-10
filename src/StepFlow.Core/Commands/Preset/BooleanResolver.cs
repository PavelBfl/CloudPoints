namespace StepFlow.Core.Commands.Preset
{
	internal class BooleanResolver<T> : IResolver<T>
	{
		public BooleanResolver(bool value) => Value = value;

		public bool Value { get; }

		public bool CanExecute(T target) => Value;
	}
}
