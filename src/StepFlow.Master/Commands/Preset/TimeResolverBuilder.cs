namespace StepFlow.Core.Commands.Preset
{
	internal sealed class TimeResolverBuilder : IResolverBuilder<Playground>
	{
		public long TimeOffset { get; set; }

		public IResolver<Playground> Build(Playground target) => new TimeResolver(target.Time + TimeOffset);
	}
}
