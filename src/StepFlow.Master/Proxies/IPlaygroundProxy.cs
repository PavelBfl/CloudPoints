using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Elements;
using StepFlow.Master.Proxies.Intersection;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy : IProxyBase<Playground>
	{
		IPlayerCharacterProxy? PlayerCharacter { get; set; }
		IList<IObstructionProxy> Obstructions { get; }
		IList<IProjectileProxy> Projectiles { get; }
		IList<IItemProxy> Items { get; }
		IList<IEnemyProxy> Enemies { get; }
		IContextProxy IntersectionContext { get; }

		void CreateObstruction(Rectangle bounds, int? strength);
		void CreatePlayerCharacter(Rectangle bounds, int strength);
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
		void CreateDamageItem(Rectangle bounds, int value, DamageKind kind);
		void CreateEnemy(Rectangle bounds, Rectangle vision);
		void CreateSpeedItem(Rectangle bounds, int speed);
	}
}
