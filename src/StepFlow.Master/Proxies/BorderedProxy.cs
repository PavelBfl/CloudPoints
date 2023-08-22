using System.Drawing;
using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public sealed class BorderedProxy : ProxyBase<Bordered>
	{
		public BorderedProxy(PlayMaster owner, Bordered target) : base(owner, target)
		{
		}

		public Cell AddCell(Rectangle border) => Target.AddCell(border);

		public void Offset(Point value) => Target.Offset(value);
	}
}
