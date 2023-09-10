using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public sealed class CellProxy : ProxyBase<Cell>, ICellProxy
	{
		public CellProxy(PlayMaster owner, Cell target) : base(owner, target)
		{
		}
	}
}
