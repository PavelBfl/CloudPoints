using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlayerCharacterProxy : IProxyBase<PlayerCharacter>, IMaterialProxy
	{
		
	}

	internal class PlayerCharacterProxy : MaterialProxy<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
		}
	}
}
