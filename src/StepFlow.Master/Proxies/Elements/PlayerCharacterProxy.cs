using System;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlayerCharacterProxy : IProxyBase<PlayerCharacter>, IMaterialProxy
	{
		new IScaleProxy Strength { get; }
	}

	internal sealed class PlayerCharacterProxy : MaterialProxy<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
			base.Strength = (IScaleProxy)Owner.CreateProxy(new Scale(Target.Context)
			{
				Value = 1,
				Max = 1,
			});
		}

		public new IScaleProxy Strength => base.Strength ?? throw new InvalidOperationException();

		public override void OnTick()
		{
			base.OnTick();

			if (Strength.Value == 0)
			{
				Owner.GetPlaygroundProxy().PlayerCharacter = null;
			}
		}
	}
}
