using System.Collections.Generic;
using StepFlow.GamePlay.Commands;

namespace StepFlow.GamePlay
{
    public interface IParticle
	{
		World Owner { get; }
		Strength Strength { get; }
		IList<Command> Commands { get; }
	}
}
