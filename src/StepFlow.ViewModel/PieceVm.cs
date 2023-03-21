﻿using System;
using StepFlow.Core;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class PieceVm : ParticleVm, IMarkered
	{
		public PieceVm(ContextVm owner, GamePlay.Piece source)
			: base(owner, source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public new GamePlay.Piece Source { get; }

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
				if (Current != value)
				{
					stateToken?.Dispose();

					current = value;
					Source.Current = Current?.Source;

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
				if (Next != value)
				{
					next = value;
					Source.Next = Next?.Source;
				}
			}
		}

		private NodeVm? GetNode(Node? node) => node is { } ? (NodeVm)WrapperProvider.GetViewModel(node) : null;

		public override void Refresh()
		{
			base.Refresh();

			Current = GetNode(Source.Current);
			Next = GetNode(Source.Next);
		}

		public override void Dispose()
		{
			CommandQueue.Clear();
			IsMark = false;
			Next = null;
			Current = null;

			base.Dispose();
		}

		public void Add(CommandVm command)
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

			Add(new MoveCommand(this, this, node));
		}
	}
}
