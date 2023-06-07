namespace StepFlow.Core.Commands.Preset
{
	internal class CreateCommandBuilder : IBuilder<Playground>
	{
		public ITargetingCommand<Playground> Build(Playground target) => new CreateCommand(target);

		public bool CanBuild(Playground target) => true;
	}
}
