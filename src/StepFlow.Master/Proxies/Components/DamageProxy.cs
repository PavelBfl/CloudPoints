using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IDamageProxy : IProxyBase<Damage>
	{
		int Value { get; set; }

		DamageKind Kind { get; set; }
	}

	internal sealed class DamageProxy : ProxyBase<Damage>, IDamageProxy
	{
		public DamageProxy(PlayMaster owner, Damage target) : base(owner, target)
		{
		}

		public int Value { get => Target.Value; set => SetValue(x => x.Value, value); }

		public DamageKind Kind { get => Target.Kind; set => SetValue(x => x.Kind, value); }
	}
}
