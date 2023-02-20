using System;

namespace StepFlow.Core.Exceptions
{
	public class InvalidCoreException : InvalidOperationException
	{
		private const string ADD_EXISTS_ELEMENT_MESSAGE = "Add to node already exists occupier element.";
		private const string REMOVE_NOT_EXISTS_ELEMENT_MESSAGE = "Remove from node not exists element.";
		private const string ADD_ALREADY_EXISTS_PARTICLE_MESSAGE = "Add already exists particle to world.";
		private const string ADD_PARTICLE_BELONG_OTHER_OWNER_MESSAGE = "Add particle belong other world.";
		private const string INVALID_ACCESS_OWNER_MESSAGE = "Invalid access owner.";
		private const string PARTICLE_WITHOUT_OWNER_MESSAGE = "Particle without owner.";

		internal static InvalidCoreException CreateAddExistsElement() => new InvalidCoreException(ADD_EXISTS_ELEMENT_MESSAGE);

		internal static InvalidCoreException CreateNotExistsElement() => new InvalidCoreException(REMOVE_NOT_EXISTS_ELEMENT_MESSAGE);

		internal static InvalidCoreException CreateAddAlreadyExistsParticle() => new InvalidCoreException(ADD_ALREADY_EXISTS_PARTICLE_MESSAGE);

		internal static InvalidCoreException CreateAddParticleBelongOtherOwner() => new InvalidCoreException(ADD_PARTICLE_BELONG_OTHER_OWNER_MESSAGE);

		internal static InvalidCoreException CreateInvalidAccessOwner() => new InvalidCoreException(INVALID_ACCESS_OWNER_MESSAGE);

		internal static InvalidCoreException CreateParticleWithoutOwner() => new InvalidCoreException(PARTICLE_WITHOUT_OWNER_MESSAGE);

		public InvalidCoreException(string? message) : base(message)
		{
		}
	}
}
