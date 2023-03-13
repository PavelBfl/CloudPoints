using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay
{
	public class Context
	{
		public Context(int colsCount, int rowsCount)
		{
			World = new World(colsCount, rowsCount);
		}

		public World World { get; }

		public Axis<Command> AxisTime { get; } = new Axis<Command>();

		private CommandsCollection CommandsCollection { get; } = new CommandsCollection();

		public IReadOnlyDictionary<Particle?, IReadOnlyList<Command>> Commands => CommandsCollection;
	}
}
