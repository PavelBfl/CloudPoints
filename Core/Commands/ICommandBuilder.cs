using System;
using System.Diagnostics.CodeAnalysis;
using StepFlow.TimeLine;

namespace StepFlow.Core.Commands
{
	public interface ICommandBuilder<out T>
		where T : ICommand
	{
		[return: MaybeNull]
		T Create(string commandName);
	}

	public interface ITargetingCommand : ICommand
	{
		Particle Target { get; }
	}

	public interface ITargetingBuilder<out T> : ICommandBuilder<T>
		where T : ITargetingCommand
	{
		Particle? Target { get; set; }
	}

	public interface INodedCommand : ITargetingCommand
	{
		HexNode Node { get; }
	}

	public interface INodedBuilder<out T> : ITargetingBuilder<T>
		where T : INodedCommand
	{
		HexNode? Node { get; set; }
	}

	public class NodedCommand : CommandBase, INodedCommand
	{
		public NodedCommand(HexNode node, Particle target)
		{
			Node = node ?? throw new ArgumentNullException(nameof(node));
			Target = target ?? throw new ArgumentNullException(nameof(target));
		}

		public HexNode Node { get; }

		public Particle Target { get; }
	}

	public class NodeBuilder : INodedBuilder<NodedCommand>
	{
		public HexNode? Node { get; set; }
		public Particle? Target { get; set; }

		[return: MaybeNull]
		public NodedCommand Create(string commandName) => commandName switch
		{
			MoveCommand.NAME => new NodedCommand(
				Node ?? throw new PropertyRequiredException(nameof(Node)),
				Target ?? throw new PropertyRequiredException(nameof(Target))
			),
			_ => null,
		};
	}

	public class PropertyRequiredException : InvalidOperationException
	{
		private const string DEFAULT_MESSAGE = "Property required for setting.";

		public PropertyRequiredException(string propertyName, string? message = null)
			: base(message ?? DEFAULT_MESSAGE)
		{
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
		}

		public string PropertyName { get; }
	}

	public class MoveCommand : NodedCommand
	{
		public const string NAME = "Move";

		public MoveCommand(HexNode node, Particle target) : base(node, target)
		{
		}

		public override void Execute()
		{
			((Piece)Target).Current = Node;
		}
	}
}
