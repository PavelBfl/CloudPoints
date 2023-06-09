using System;

namespace StepFlow.Core.Commands.Preset
{
	internal class CreateCommandBuilder : IBuilder<Playground>
	{
		public CreateCommandBuilder(IResolver<Playground> resolver) => Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

		public IResolver<Playground> Resolver { get; }

		public ITargetingCommand<Playground> Build(Playground target) => new CreateCommand(target, Resolver);

		public bool CanBuild(Playground target) => true;
	}
}
