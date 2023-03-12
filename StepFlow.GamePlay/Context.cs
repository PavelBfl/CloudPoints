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
	}
}
