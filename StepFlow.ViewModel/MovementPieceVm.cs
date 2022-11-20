using System.Collections.Generic;
using System;
using StepFlow.Core;
using StepFlow.TimeLine;
using System.Collections.Specialized;
using System.Collections;

namespace StepFlow.ViewModel
{
	public interface IPieceVm
	{
		CommandsQueueVm CommandQueue { get; }
	}

	public interface IMovementPieceVm : IPieceVm
	{
		void MoveTo(HexNodeVm node);
	}

	public class PieceVm<T> : WrapperVm<T>, IPieceVm
		where T : Piece
	{
		public PieceVm(WorldVm world, T source) : base(source, true)
		{
			Owner = world ?? throw new ArgumentNullException(nameof(world));
			CommandQueue = new CommandsQueueVm(Source);
		}

		public WorldVm Owner { get; }

		public CommandsQueueVm CommandQueue { get; }

	}
	public class CommandsQueueVm : IReadOnlyList<ICommand>, INotifyCollectionChanged
		{
			public CommandsQueueVm(Piece source)
			{
				Source = source ?? throw new ArgumentNullException(nameof(source));
			}

			public ICommand this[int index] => Source.CommandsQueue[index];

			public int Count => Source.CommandsQueue.Count;

			private Piece Source { get; }

			public event NotifyCollectionChangedEventHandler? CollectionChanged;

			public void Add(ICommand command) => Source.Add(command);

			public IEnumerator<ICommand> GetEnumerator() => Source.CommandsQueue.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

	public class MovementPieceVm : PieceVm<MovementPiece>, IMovementPieceVm
	{
		public MovementPieceVm(WorldVm owner, HexNodeVm hexNode)
			: base(owner, new MovementPiece(owner.Source, hexNode.Source))
		{
			current = hexNode;

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
