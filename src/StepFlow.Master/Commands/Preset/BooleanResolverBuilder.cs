namespace StepFlow.Core.Commands.Preset
{
	internal class BooleanResolverBuilder<T> : IResolverBuilder<T>
	{
		public BooleanResolverBuilder(bool value) => Value = value;

		public bool Value { get; }

		public IResolver<T> Build(T target) => new BooleanResolver<T>(Value);
	}
}
