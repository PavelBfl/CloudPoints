using System;
using StepFlow.GamePlay;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class AxisVm : WrapperVm<Axis<Command>>
	{
		public AxisVm(IContextElement context, Axis<Command> source)
			: base(context, source)
		{
		}

		public long NearestAllow => Source.NearestAllow;

		public bool TryGetTime(Command command, out long result) => Source.TryGetTime(command, out result);

		public void Remove(CommandVm command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			Source.Remove(command.Source);
		}

		public void MoveNext() => Source.MoveNext();
	}
}
