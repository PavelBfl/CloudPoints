using System;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay
{
	public class Context
	{
		public Context(int colsCount, int rowsCount)
		{
			World = new World(colsCount, rowsCount);
		}

		public World World { get; }

		public Axis<Command> AxisTime { get; } = new Axis<Command>();

		public IList<Command> StaticCommands { get; } = new List<Command>();
	}

	public interface IParticle
	{
		Context Owner { get; }
		Strength Strength { get; }
		IList<Command> Commands { get; }
	}

	public class Node : Core.Node, IParticle
	{
		public Node(Context owner, Point position, float strengthDefault = 0)
			: base(owner.World, position)
		{
			Owner = owner;
			Strength = new Strength(strengthDefault);
		}

		public Context Owner { get; }

		public Strength Strength { get; }

		public IList<Command> Commands { get; } = new List<Command>();
	}

	public class Piece : Core.Piece, IParticle
	{
		public Piece(Context owner, float strengthDefault = 0)
			: base(owner.World)
		{
			Owner = owner;
			Strength = new Strength(strengthDefault);
		}

		public Context Owner { get; }

		public Strength Strength { get; }

		public IList<Command> Commands { get; } = new List<Command>();

		public double CollisionDamage { get; set; }
	}

	public class Strength
	{
		public Strength(float max)
			: this(max, max)
		{
		}

		public Strength(float value, float max)
		{
			this.value = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
			this.max = max >= 0 ? max : throw new ArgumentOutOfRangeException(nameof(max));
		}

		private float value;

		public float Value
		{
			get => value;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this.value = MathF.Min(value, Max);
			}
		}

		private float max;

		public float Max
		{
			get => max;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				max = value;
				Value = MathF.Min(Value, Max);
			}
		}
	}
}
