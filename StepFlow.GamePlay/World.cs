using System;

namespace StepFlow.GamePlay
{
	public sealed class World : Core.World<Node, Piece>
	{
		public World(Context owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public Context Owner { get; }
	}
}
