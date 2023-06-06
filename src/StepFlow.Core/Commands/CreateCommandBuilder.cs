namespace StepFlow.Core.Commands
{
	internal class CreateCommandBuilder : IBuilder<Playground>
	{
		public ITargetingCommand<Playground> Build(Playground target) => new CreateCommand(target, new Strength(100), 100, null);

		public bool CanBuild(Playground target) => true;
	}
}
