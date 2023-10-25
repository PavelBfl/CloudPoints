using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	internal sealed class CellProxy : ProxyBase<Cell>, ICellProxy
	{
		public CellProxy(PlayMaster owner, Cell target) : base(owner, target)
		{
		}
	}
}
