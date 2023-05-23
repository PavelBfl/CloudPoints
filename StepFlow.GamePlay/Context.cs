using System.Collections.Generic;
using StepFlow.GamePlay.Commands;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay
{
	public class Context
	{
		public Context()
		{
			World = new World(this);
		}

		public World World { get; }

		public Axis<Command> AxisTime { get; } = new Axis<Command>();

		public IList<Command> StaticCommands { get; } = new List<Command>();
	}
}
