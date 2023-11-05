using System.Drawing;
using StepFlow.Core.Border;

namespace StepFlow.Master.Proxies
{
	public interface IBorderedProxy : IProxyBase<Bordered>
	{
		ICellProxy AddCell(Rectangle border);

		void Offset(Point value);
	}
}
