using System;

namespace StepFlow.Core.Components
{
	[Flags]
	public enum SubjectKind
	{
		None = 0,
		PlayerCharacter = 1,
		Projectile = 2,
		Wall = 4,
	}

	public sealed class State : ComponentBase
	{
		public State(Playground owner) : base(owner)
		{
		}

		public int Team { get; set; }

		public SubjectKind Kind { get; set; }
	}
}
