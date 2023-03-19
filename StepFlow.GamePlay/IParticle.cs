using System.Collections.Generic;

namespace StepFlow.GamePlay
{
	public interface IParticle
	{
		Context Owner { get; }
		Strength Strength { get; }
		IList<Command> Commands { get; }
	}
}
