using System.Collections.Generic;
using System;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public interface IPieceVm
	{
		void MoveTo(HexNodeVm node);
	}

	public class MovementPieceVm : WrapperVm<MovementPiece>, IPieceVm
	{
		public MovementPieceVm(WorldVm world, HexNodeVm hexNode)
			: base(new MovementPiece(world.Source, hexNode.Source), true)
		{
			current = hexNode;
			Owner = world ?? throw new ArgumentNullException(nameof(world));

			Current.SetCurrent();
		}

		private HexNodeVm current;
		public HexNodeVm Current
		{
			get => current;
			set
			{
				if (Current != value)
				{
					Current.SetNode();
					current = value;
					Current.SetCurrent();
					Source.Current = Current.Source;
				}
			}
		}

		public WorldVm Owner { get; }

		public IReadOnlyList<ICommand> CommandQueue => Source.CommandsQueue;

		public void MoveTo(HexNodeVm node)
		{
			Source.Enqueue(new MoveCommand(this, node));
		}

		private class MoveCommand : CommandBase
		{
			public MoveCommand(MovementPieceVm owner, HexNodeVm nextNode)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				NextNode = nextNode ?? throw new ArgumentNullException(nameof(nextNode));
			}

			public MovementPieceVm Owner { get; }

			public HexNodeVm NextNode { get; }

			public override void Execute()
			{
				Owner.Current = NextNode;
			}
		}
	}
}
