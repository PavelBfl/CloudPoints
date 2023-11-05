using System.Drawing;
using StepFlow.Core.Border;

namespace StepFlow.Master.Proxies
{
	internal sealed class BorderedProxy : ProxyBase<Bordered>, IBorderedProxy
	{
		public BorderedProxy(PlayMaster owner, Bordered target) : base(owner, target)
		{
		}

		public ICellProxy AddCell(Rectangle border)
		{
			var cell = new Cell()
			{
				Border = border,
			};

			Target.Childs.Add(cell);
			return (ICellProxy)Owner.CreateProxy(cell);
		}

		public void Offset(Point value) => Target.Offset(value);
	}
}
