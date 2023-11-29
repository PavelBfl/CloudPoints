using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy : IProxyBase<Playground>
	{
		IPlayerCharacterProxy? PlayerCharacter { get; set; }
		IList<IObstructionProxy> Obstructions { get; }
		IList<IProjectileProxy> Projectiles { get; }
		IList<IItemProxy> Items { get; }
		IList<IEnemyProxy> Enemies { get; }

		void CreateObstruction(Rectangle bounds, int? strength);
		IBorderedProxy CreateBordered();
		ICellProxy CreateCell(Rectangle border);
		void CreatePlayerCharacter(Rectangle bounds, int strength);
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
		void CreateDamageItem(Rectangle bounds, int value, DamageKind kind);
		void CreateEnemy(Rectangle bounds, Rectangle vision);
		void CreateSpeedItem(Rectangle bounds, int speed);
	}
}
