using System;

namespace Core
{
	public class Particle
	{
		public Particle(World owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public World Owner { get; }
	}
}
