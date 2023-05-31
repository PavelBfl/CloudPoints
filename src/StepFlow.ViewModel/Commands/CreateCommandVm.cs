using System.Collections.Generic;

namespace StepFlow.ViewModel.Commands
{
	public sealed class CreateCommandVm : CommandVm
	{
		public CreateCommandVm(WrapperProvider wrapperProvider, GamePlay.Commands.CreateCommand source)
			: base(wrapperProvider, source)
		{
			Source = source;
		}

		private new GamePlay.Commands.CreateCommand Source { get; }

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
