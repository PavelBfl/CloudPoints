using System.Drawing;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy
	{
		IPlayerCharacterProxy? PlayerCharacter { get; set; }

		IBorderedProxy CreateBordered();
		ICellProxy CreateCell(Rectangle border);
		void CreatePlayerCharacter(Rectangle bounds, float strength);
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
	}
}
