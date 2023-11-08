using System.Drawing;
using StepFlow.Master.Proxies.Border;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy
	{
		IBorderedProxy CreateBordered();
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
	}
}
