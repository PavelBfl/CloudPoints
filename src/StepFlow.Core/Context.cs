using StepFlow.Core.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public class Context
	{
		public Playground Playground { get; } = new Playground();

		public Axis<ITargetingCommand<object>> AxisTime { get; } = new Axis<ITargetingCommand<object>>();
	}
}
