using StepFlow.Core;

namespace StepFlow.Master
{
	public interface IPieceCmd : IParticleCmd<Piece>
	{
		INodeCmd? Current { get; set; }

		INodeCmd? Next { get; set; }

		bool IsScheduledStep { get; set; }

		float CollisionDamage { get; set; }
	}
}
