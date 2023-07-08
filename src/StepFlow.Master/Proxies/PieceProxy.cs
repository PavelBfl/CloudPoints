using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Commands.Accessors;

namespace StepFlow.Master.Proxies
{
	public sealed class PieceProxy : ParticleProxy<Piece>
	{
		[MoonSharpHidden]
		public PieceProxy(PlayMaster owner, Piece source) : base(owner, source)
		{
		}

		public Node? Current { get => Target.Current; set => Owner.TimeAxis.Add(Target.CreatePropertyCommand(x => x.Current, value)); }

		public Node? Next { get => Target.Next; set => Owner.TimeAxis.Add(Target.CreatePropertyCommand(x => x.Next, value)); }

		public bool IsScheduledStep { get => Target.IsScheduledStep; set => Owner.TimeAxis.Add(Target.CreatePropertyCommand(x => x.IsScheduledStep, value)); }

		public float CollisionDamage { get => Target.CollisionDamage; set => Owner.TimeAxis.Add(Target.CreatePropertyCommand(x => x.CollisionDamage, value)); }
	}
}
