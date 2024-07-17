using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.States;

namespace StepFlow.Core.States
{
	public sealed class State : Subject
	{
		public State(IContext context)
			: base(context)
		{
		}

		public State(IContext context, StateDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Kind = original.Kind;
			Enable = original.Enable;
			Cooldown = original.Cooldown;
			TotalCooldown = original.TotalCooldown;
			Arg0 = original.Arg0;
			Arg1 = original.Arg1;
		}

		public StateKind Kind { get; set; }

		public bool Enable { get; set; } = true;

		public Scale Cooldown { get; set; }

		public int? TotalCooldown { get; set; }

		public float Arg0 { get; set; }

		public float Arg1 { get; set; }

		public override SubjectDto ToDto()
		{
			var result = new StateDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(StateDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Kind = Kind;
			container.Enable = Enable;
			container.Cooldown = Cooldown;
			container.TotalCooldown = TotalCooldown;
			container.Arg0 = Arg0;
			container.Arg1 = Arg1;
		}

		public override bool Equals(object obj) => obj is State other && Kind == other.Kind;

		public override int GetHashCode() => Kind.GetHashCode();

		public override string ToString() => Kind.ToString();
	}
}
