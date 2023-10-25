using System.Drawing;
using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	internal sealed class BorderedProxy : ProxyBase<Bordered>, IBorderedProxy
	{
		public BorderedProxy(PlayMaster owner, Bordered target) : base(owner, target)
		{
		}

		public ICellProxy AddCell(Rectangle border) => (ICellProxy)Owner.CreateProxy(Target.AddCell(border));

		public void Offset(Point value) => Target.Offset(value);
	}
}
