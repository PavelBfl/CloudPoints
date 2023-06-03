using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Commands;

namespace StepFlow.ViewModel.Commands
{
	public sealed class CreateCommandVm : CommandVm<CreateCommand, Playground, PlaygroundVm>
	{
		public CreateCommandVm(WrapperProvider wrapperProvider, CreateCommand source)
			: base(wrapperProvider, source)
		{
		}

		private NodeVm? begin;

		public NodeVm? Begin => begin ??= WrapperProvider.Get<NodeVm?>(Source.Begin);

		private PieceVm? piece;

		public PieceVm? Piece => piece ??= WrapperProvider.GetOrCreate<PieceVm>(Source.Piece);

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

		public override IEnumerable<IWrapper> GetContent() => base.GetContent().ConcatIfNotNull(begin, piece);
	}
}
