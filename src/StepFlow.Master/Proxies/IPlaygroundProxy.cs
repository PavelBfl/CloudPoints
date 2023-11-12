using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy : IProxyBase<Playground>
	{
		IPlayerCharacterProxy? PlayerCharacter { get; set; }
		IList<IObstructionProxy> Obstructions { get; }
		IList<IProjectileProxy> Projectiles { get; }

		void CreateObstruction(Rectangle bounds, int? strength);
		IBorderedProxy CreateBordered();
		ICellProxy CreateCell(Rectangle border);
		void CreatePlayerCharacter(Rectangle bounds, int strength);
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
	}
}
