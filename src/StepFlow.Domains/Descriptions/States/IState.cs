using StepFlow.Common;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;

namespace StepFlow.Domains.Descriptions.States
{
	public interface IState : ISubject, IClonerTo<IState>
	{
		public static void CopyTo(IState source, IState destination)
		{
			NullValidate.ThrowIfArgumentNull(source, nameof(source));
			NullValidate.ThrowIfArgumentNull(destination, nameof(destination));

			ISubject.CopyTo(source, destination);

			destination.Kind = source.Kind;
			destination.Enable = source.Enable;
			destination.Cooldown = source.Cooldown;
			destination.TotalCooldown = source.TotalCooldown;
			destination.Arg0 = source.Arg0;
			destination.Arg1 = source.Arg1;
		}

		void IClonerTo<IState>.CloneTo(IState container) => CopyTo(this, container);

		StateKind Kind { get; set; }

		bool Enable { get; set; }

		Scale Cooldown { get; set; }

		int? TotalCooldown { get; set; }

		float Arg0 { get; set; }

		float Arg1 { get; set; }
	}
}
