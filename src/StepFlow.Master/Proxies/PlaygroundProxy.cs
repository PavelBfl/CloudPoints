using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Elements;
using StepFlow.Master.Proxies.Intersection;

namespace StepFlow.Master.Proxies
{
	internal sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public IContextProxy Context => (IContextProxy)Owner.CreateProxy(Context);

		public IPlayerCharacterProxy? PlayerCharacter { get => (IPlayerCharacterProxy?)Owner.CreateProxy(Target.PlayerCharacter); set => SetValue(x => x.PlayerCharacter, ((IProxyBase<PlayerCharacter>?)value)?.Target); }

		public IList<IObstructionProxy> Obstructions => new ListItemsProxy<Obstruction, IList<Obstruction>, IObstructionProxy>(Owner, Target.Obstructions);

		public IList<IProjectileProxy> Projectiles => new ListItemsProxy<Projectile, IList<Projectile>, IProjectileProxy>(Owner, Target.Projectiles);

		public IList<IItemProxy> Items => CreateListItemProxy<Item, IItemProxy>(Target.Items);

		public IList<IEnemyProxy> Enemies => CreateListItemProxy<Enemy, IEnemyProxy>(Target.Enemies);

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

		public IShapeContainerProxy CreateShapeContainer(params Rectangle[] rectangles)
			=> CreateShapeContainer(rectangles.AsEnumerable());

		public IShapeContainerProxy CreateShapeContainer(IEnumerable<Rectangle> subRectangles)
		{
			var shape = (IShapeContainerProxy)Owner.CreateProxy(new ShapeContainer(subRectangles));
			Context.Add(shape);
			return shape;
		}

		public IShapeCellProxy CreateCell(Rectangle border)
		{
			var shape = (IShapeCellProxy)Owner.CreateProxy(new ShapeCell(border));
			Context.Add(shape);
			return shape;
		}

		public void CreatePlayerCharacter(Rectangle bounds, int strength)
		{
			PlayerCharacter = (IPlayerCharacterProxy)Owner.CreateProxy(new PlayerCharacter()
			{
				Strength = new Scale()
				{
					Max = strength,
					Value = strength,
				},
				Cooldown = new Scale()
				{
					Max = 1000,
					Value = 1000,
				},
				Body = new Collided()
				{
					Current = CreateCell(bounds).Target,
					IsRigid = true,
				},
				Speed = 1,
			});
		}

		public void CreateObstruction(Rectangle bounds, int? strength)
		{
			var barrier = (IObstructionProxy)Owner.CreateProxy(new Obstruction()
			{
				Body = new Collided()
				{
					Current = CreateCell(bounds).Target,
					IsRigid = true,
				},
				Strength = strength is { } ?
					new Scale()
					{
						Max = strength.Value,
						Value = strength.Value,
					} :
					null,
			});

			Obstructions.Add(barrier);
		}

		public void CreateProjectile(Rectangle bounds, int value, DamageKind kind)
		{
			var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile()
			{
				Body = new Collided()
				{
					Current = CreateCell(bounds).Target,
					IsRigid = true,
				},
				Damage = new Damage()
				{
					Value = value,
					Kind = kind,
				},
				Speed = 5,
			});

			Projectiles.Add(projectile);
		}

		public void CreateDamageItem(Rectangle bounds, int value, DamageKind kind)
		{
			var item = (IItemProxy)Owner.CreateProxy(new Item()
			{
				Kind = kind switch
				{
					DamageKind.Fire => ItemKind.Fire,
					DamageKind.Poison => ItemKind.Poison,
					_ => ItemKind.None,
				},
				Body = new Collided()
				{
					Current = Owner.GetPlaygroundProxy().CreateCell(bounds).Target,
					IsRigid = true,
				},
				DamageSetting = new Damage()
				{
					Value = value,
					Kind = kind,
				},
			});

			Items.Add(item);
		}

		public void CreateSpeedItem(Rectangle bounds, int speed)
		{
			var item = (IItemProxy)Owner.CreateProxy(new Item()
			{
				Kind = ItemKind.Speed,
				Body = new Collided()
				{
					Current = Owner.GetPlaygroundProxy().CreateCell(bounds).Target,
					IsRigid = true,
				},
				Speed = speed,
			});

			Items.Add(item);
		}

		public void CreateEnemy(Rectangle bounds, Rectangle vision)
		{
			var enemy = (IEnemyProxy)Owner.CreateProxy(new Enemy()
			{
				Body = new Collided()
				{
					Current = Owner.GetPlaygroundProxy().CreateCell(bounds).Target,
					IsRigid = true,
				},
				Vision = new Collided()
				{
					Current = Owner.GetPlaygroundProxy().CreateCell(vision).Target,
				},
				Cooldown = new Scale()
				{
					Value = 1000,
					Max = 1000,
				},
				Strength = new Scale()
				{
					Value = 100,
					Max = 100,
				},
			});

			Enemies.Add(enemy);
		}
	}
}
