namespace StepFlow.Core
{
	public class Particle : Subject
	{
		public Particle(Playground owner) : base(owner)
		{
		}

		public Strength Strength { get; } = new Strength();
	}
}
