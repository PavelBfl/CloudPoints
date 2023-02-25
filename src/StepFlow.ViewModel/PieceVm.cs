using System;
using StepFlow.Common;
using StepFlow.Core;
using StepFlow.Entities;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class PieceVm : ParticleVm<Piece>, IParticleVm, IMarkered
	{
		public PieceVm(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			CommandQueue = new CommandsQueueVm(this);
		}

		private bool isMark = false;
		public bool IsMark
		{
			get => isMark;
			set
			{
				if (IsMark != value)
				{
					isMark = value;

					CommandQueue.IsMark = IsMark;

					if (Current is { })
					{
						Current.IsMark = IsMark;
					}
				}
			}
		}

		private IDisposable? stateToken;

		private NodeVm? current;
		public NodeVm? Current
		{
			get => current;
			set
			{
				var source = UsePropertySourceRequired();
				if (Current != value)
				{
					stateToken?.Dispose();

					current = value;
					source.Current = Current?.Source;

					stateToken = Current?.State.Registry(NodeState.Current);
				}
			}
		}

		private NodeVm? next;
		public NodeVm? Next
		{
			get => next;
			set
			{
				var source = UsePropertySourceRequired();

				if (Next != value)
				{
					next = value;
					source.Next = Next?.Source;
				}
			}
		}

		public void TakeStep()
		{
			var source = UseMethodSourceRequired();
			var owner = Owner.PropertyRequired(nameof(Owner));

			var newCurrent = source.Current is { } ? (NodeVm)owner.Particles[source.Current] : null;
			Current = newCurrent;

			Next = null;
		}

		public CommandsQueueVm CommandQueue { get; }

		public void Add(ICommandVm command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			command.IsMark = IsMark;
			CommandQueue.Add(command);
		}

		public void MoveTo(NodeVm node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			Add(new MoveCommand(this, node));
		}
	}
}
