using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public sealed class CellProxy : ProxyBase<Cell>
	{
		public CellProxy(PlayMaster owner, Cell target) : base(owner, target)
		{
		}
	}
}
