using System;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;
using StepFlow.ViewModel.Exceptions;

namespace StepFlow.ViewModel
{
	public class CommandsQueueVm : ListWrapperObserver<CommandVm, GamePlay.Command>, IMarkered
	{
		public CommandsQueueVm(PieceVm owner)
			: base(owner.Source.Commands)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public PieceVm Owner { get; }

		private bool isMark;

		public bool IsMark
		{
			get => isMark;
			set
			{
				if (IsMark != value)
				{
					isMark = value;

					foreach (var command in this)
					{
						command.IsMark = IsMark;
					}
				}
			}
		}

		protected override CommandVm CreateObserver(GamePlay.Command observable)
		{
			if (Owner.WrapperProvider.TryGetViewModel(observable, out var result))
			{
				return (CommandVm)result;
			}
			else
			{
				return observable switch
				{
					GamePlay.MoveCommand moveCommand => new MoveCommand(
						Owner,
						(PieceVm)Owner.WrapperProvider.GetViewModel(moveCommand.Target),
						(NodeVm)Owner.WrapperProvider.GetViewModel(moveCommand.Next)
					),
					_ => throw new InvalidViewModelException(),
				}; 
			}
		}
	}
}
