using System.Collections.Generic;
using StepFlow.Core;

namespace StepFlow.Master
{
	public interface INodeCmd : IParticleCmd<Node>
	{
		IReadOnlyCollection<IPieceCmd> Occupiers { get; }
	}
}
