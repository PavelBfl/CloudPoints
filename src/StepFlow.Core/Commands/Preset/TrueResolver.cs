namespace StepFlow.Core.Commands.Preset
{
	internal class TrueResolver : IResolver<object?>
	{
		public static TrueResolver Instance { get; } = new TrueResolver();

		private TrueResolver()
		{
		}

		public bool CanExecute(object? target) => true;
	}
}
