using StepFlow.Domains.Components;

namespace StepFlow.Domains.States
{
	public class StateDto : SubjectDto
	{
		public StateKind Kind { get; set; }

		public bool Enable { get; set; } = true;

		public Scale Cooldown { get; set; }

		public int? TotalCooldown { get; set; }

		public float Arg0 { get; set; }

		public float Arg1 { get; set; }

		public override bool Equals(object obj) => obj is StateDto other && Kind == other.Kind;

		public override int GetHashCode() => Kind.GetHashCode();

		public override string ToString() => Kind.ToString();
	}
}
