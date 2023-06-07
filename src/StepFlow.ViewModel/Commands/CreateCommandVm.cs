using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Commands.Preset;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Commands
{
    public sealed class CreateCommandVm : CommandVm<CreateCommand, Playground, PlaygroundVm>
	{
		public CreateCommandVm(LockProvider wrapperProvider, CreateCommand source)
			: base(wrapperProvider, source)
		{
		}

		private NodeVm? begin;

		public NodeVm? Begin => begin ??= LockProvider.Get<NodeVm?>(Source.Begin);

		private PieceVm? piece;

		public PieceVm? Piece => piece ??= LockProvider.GetOrCreate<PieceVm>(Source.Piece);

		public override bool IsMark
		{
			get => Begin?.IsMark ?? false;
			set
			{
				if (Begin is { })
				{
					Begin.IsMark = value; 
				}
			}
		}

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(begin, piece);
	}
}
