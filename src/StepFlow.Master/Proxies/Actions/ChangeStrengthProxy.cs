using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies.Actions
{
	public interface IChangeStrengthProxy : IActionBaseProxy<ChangeStrength>
	{

	}

	internal sealed class ChangeStrengthProxy : ActionBaseProxy<ChangeStrength>, IChangeStrengthProxy
	{
		public ChangeStrengthProxy(PlayMaster owner, ChangeStrength target) : base(owner, target)
		{
		}

		public Material? Material { get => Target.Material; set => SetValue(value); }

		public Damage Damage { get => Target.Damage; set => SetValue(value); }

		public override void Execute()
		{
			if ((IMaterialProxy<Material>?)Owner.CreateProxy(Material) is { } materialProxy)
			{
				materialProxy.ChangeStrength(Damage);
			}
		}
	}
}
