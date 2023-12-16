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

		public IContextProxy IntersectionContext => (IContextProxy)Owner.CreateProxy(Target.IntersectionContext);

		public IPlayerCharacterProxy? PlayerCharacter { get => (IPlayerCharacterProxy?)Owner.CreateProxy(Target.PlayerCharacter); set => SetValue(x => x.PlayerCharacter, ((IProxyBase<PlayerCharacter>?)value)?.Target); }

		public IList<IObstructionProxy> Obstructions => new ListItemsProxy<Obstruction, IList<Obstruction>, IObstructionProxy>(Owner, Target.Obstructions);

		public IList<IProjectileProxy> Projectiles => new ListItemsProxy<Projectile, IList<Projectile>, IProjectileProxy>(Owner, Target.Projectiles);

		public IList<IItemProxy> Items => CreateListItemProxy<Item, IItemProxy>(Target.Items);

		public IList<IEnemyProxy> Enemies => CreateListItemProxy<Enemy, IEnemyProxy>(Target.Enemies);

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

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
					IsRigid = true,
				},
				Speed = 1,
			});

			PlayerCharacter.Body.Current = (IShapeBaseProxy<ShapeBase>)Owner.CreateProxy(new ShapeCell(bounds));
		}

		public void CreateObstruction(Rectangle bounds, int? strength)
		{
			var barrier = (IObstructionProxy)Owner.CreateProxy(new Obstruction()
			{
				Body = new Collided()
				{
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

			barrier.Body.Current = (IShapeBaseProxy<ShapeBase>)Owner.CreateProxy(new ShapeCell(bounds));

			Obstructions.Add(barrier);
		}

		public void CreateProjectile(Rectangle bounds, int value, DamageKind kind)
		{
			var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile()
			{
				Body = new Collided()
				{
					IsRigid = true,
				},
				Damage = new Damage()
				{
					Value = value,
					Kind = kind,
				},
				Speed = 5,
			});

			projectile.Body.Current = (IShapeBaseProxy<ShapeBase>)Owner.CreateProxy(new ShapeCell(bounds));

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
					IsRigid = true,
				},
				DamageSetting = new Damage()
				{
					Value = value,
					Kind = kind,
				},
			});

			item.Body.Current = (IShapeBaseProxy<ShapeBase>)Owner.CreateProxy(new ShapeCell(bounds));

			Items.Add(item);
		}

		public void CreateSpeedItem(Rectangle bounds, int speed)
		{
			var item = (IItemProxy)Owner.CreateProxy(new Item()
			{
				Kind = ItemKind.Speed,
				Body = new Collided()
				{
					IsRigid = true,
				},
				Speed = speed,
			});

			item.Body.Current = (IShapeBaseProxy<ShapeBase>)Owner.CreateProxy(new ShapeCell(bounds));

			Items.Add(item);
		}

		public void CreateEnemy(Rectangle bounds, Rectangle vision)
		{
			var enemy = (IEnemyProxy)Owner.CreateProxy(new Enemy()
			{
				Body = new Collided()
				{
					IsRigid = true,
				},
				Vision = new Collided(),
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

			enemy.Body.Current = (IShapeBaseProxy<ShapeBase>)Owner.CreateProxy(new ShapeCell(bounds));
			enemy.Vision.Current = (IShapeBaseProxy<ShapeBase>)Owner.CreateProxy(new ShapeCell(vision));

			Enemies.Add(enemy);
		}
	}
}
