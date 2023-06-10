using System;

namespace StepFlow.Core.Commands.Preset
{
	internal class CreateCommandBuilder : IBuilder<Playground>
	{
		public CreateCommandBuilder(IResolverBuilder<Playground> resolverBuilder)
			=> ResolverBuilder = resolverBuilder ?? throw new ArgumentNullException(nameof(resolverBuilder));

		public IResolverBuilder<Playground> ResolverBuilder { get; }

		public ITargetingCommand<Playground> Build(Playground target) => new CreateCommand(target, ResolverBuilder.Build(target));

		public bool CanBuild(Playground target) => true;
	}
}
