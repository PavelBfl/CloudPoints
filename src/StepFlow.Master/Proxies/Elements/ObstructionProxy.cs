using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IObstructionProxy : IProxyBase<Obstruction>, IMaterialProxy
	{
		
	}

	internal class ObstructionProxy : MaterialProxy<Obstruction>, IObstructionProxy
	{
		public ObstructionProxy(PlayMaster owner, Obstruction target) : base(owner, target)
		{
		}
	}
}
