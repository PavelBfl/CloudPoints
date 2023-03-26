using System;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class CommandsCollectionVm : ListWrapperObserver<CommandVm, GamePlay.Command>, IMarkered
	{
		public CommandsCollectionVm(ParticleVm owner)
			: base(owner.Source.Commands)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public ParticleVm Owner { get; }

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
			=> Owner.WrapperProvider.GetOrCreateCommand(observable);
	}
}
