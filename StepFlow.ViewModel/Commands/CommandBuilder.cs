using System;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.ViewModel.Commands
{
	public abstract class CommandBuilder
	{
		protected CommandBuilder(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			Nodes = count == 0 ? Array.Empty<HexNodeVm?>() : new HexNodeVm?[count];
		}

		public List<IPieceVm> Current { get; }

		public HexNodeVm?[] Nodes { get; }

		public bool CanBuild => Nodes.All(x => x is { });

		public abstract ICommandVm Build();
	}

	public class MoveCommandBuilder : CommandBuilder
	{
		public MoveCommandBuilder()
			: base(1)
		{
		}

		public override ICommandVm Build()
		{
			if (!CanBuild)
			{
				throw new InvalidOperationException();
			}

			return new MoveCommandVm();
		}
	}
}
