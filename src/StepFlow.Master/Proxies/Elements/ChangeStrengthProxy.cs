using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IChangeStrengthProxy : IProxyBase<ChangeStrength>, ITurnExecutor
	{
		
	}

	internal class ChangeStrengthProxy : ProxyBase<ChangeStrength>, IChangeStrengthProxy
	{
		public ChangeStrengthProxy(PlayMaster owner, ChangeStrength target) : base(owner, target)
		{
		}

		public Material? Material { get => Target.Material; set => SetValue(x => x.Material, value); }

		public Damage Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }

		Subject IProxyBase<Subject>.Target => Target;

		public void Execute()
		{
			if ((IMaterialProxy<Material>?)Owner.CreateProxy(Material) is { } materialProxy)
			{
				materialProxy.ChangeStrength(Damage);
			}
		}
	}
}
