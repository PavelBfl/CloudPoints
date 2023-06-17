namespace StepFlow.Core.Commands.Preset
{
	internal class TimeResolver : IResolver<Playground>
	{
		public TimeResolver(long time) => Time = time;

		public long Time { get; }

		public bool CanExecute(Playground target) => target.Time == Time;
	}
}
