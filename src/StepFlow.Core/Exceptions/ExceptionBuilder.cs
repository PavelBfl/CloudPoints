using System;

namespace StepFlow.Core.Exceptions
{
	internal static class ExceptionBuilder
	{
		private const string ADD_EXISTS_ELEMENT_MESSAGE = "Add to node already exists occupier element.";
		private const string REMOVE_NOT_EXISTS_ELEMENT_MESSAGE = "Remove from node not exists element.";
		private const string ADD_ALREADY_EXISTS_PARTICLE_MESSAGE = "Add already exists particle to world.";
		private const string ADD_PARTICLE_BELONG_OTHER_OWNER_MESSAGE = "Add particle belong other world.";
		private const string INVALID_ACCESS_OWNER_MESSAGE = "Invalid access owner.";
		private const string PARTICLE_WITHOUT_OWNER_MESSAGE = "Particle without owner.";

		internal static CoreException CreateAddExistsElement() => new CoreException(ADD_EXISTS_ELEMENT_MESSAGE);

		internal static CoreException CreateNotExistsElement() => new CoreException(REMOVE_NOT_EXISTS_ELEMENT_MESSAGE);

		internal static CoreException CreateAddAlreadyExistsParticle() => new CoreException(ADD_ALREADY_EXISTS_PARTICLE_MESSAGE);

		internal static CoreException CreateAddParticleBelongOtherOwner() => new CoreException(ADD_PARTICLE_BELONG_OTHER_OWNER_MESSAGE);

		internal static CoreException CreateInvalidAccessOwner() => new CoreException(INVALID_ACCESS_OWNER_MESSAGE);

		internal static CoreException CreateParticleWithoutOwner() => new CoreException(PARTICLE_WITHOUT_OWNER_MESSAGE);

		internal static CoreException CreateParticleCanNotInteraction() => new CoreException("Particle can not for interaction.");

		internal static CoreException CreatePairParticlesCanNotInteraction() => new CoreException("Pair particles can not for interaction.");

		internal static Exception CreatePropertyIsNull(string? propertyName) => new PropertyNullException(propertyName);
	}
}
