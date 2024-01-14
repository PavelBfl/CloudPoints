using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;
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

		public void CreatePlayerCharacter(Rectangle bounds, int strength)
		{
			PlayerCharacter = (IPlayerCharacterProxy)Owner.CreateProxy(new PlayerCharacter()
			{
				Name = "Player",
				Strength = new Scale()
				{
					Max = strength,
					Value = strength,
				},
				Cooldown = new Scale()
				{
					Max = 3000,
					Value = 3000,
				},
				Body = new Collided()
				{
					Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
					IsRigid = true,
				},
				Speed = 10,
			});
		}

		public void CreateObstruction(Rectangle bounds, int? strength)
		{
			var barrier = (IObstructionProxy)Owner.CreateProxy(new Obstruction()
			{
				Name = "Obstruction",
				Body = new Collided()
				{
					Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
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
					Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
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

		public void CreateEnemy(Rectangle bounds, Rectangle vision)
		{
			var enemy = (IEnemyProxy)Owner.CreateProxy(new Enemy()
			{
				Body = new Collided()
				{
					Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
					IsRigid = true,
				},
				Vision = new Collided()
				{
					Current = new ShapeCell(Owner.Playground.IntersectionContext, vision),
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

		public void CreateItem(Point position, ItemKind kind)
		{
			const int RADIUS = 7;

			var bounds = new Rectangle(
				position.X - RADIUS,
				position.Y - RADIUS,
				RADIUS * 2,
				RADIUS * 2
			);

			var item = kind switch
			{
				ItemKind.Fire => (IItemProxy)Owner.CreateProxy(new Item()
				{
					Kind = ItemKind.Fire,
					Body = new Collided()
					{
						Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
						IsRigid = true,
					},
					DamageSetting = new Damage()
					{
						Value = 10,
						Kind = DamageKind.Fire,
					},
				}),
				ItemKind.Poison => (IItemProxy)Owner.CreateProxy(new Item()
				{
					Kind = ItemKind.Poison,
					Body = new Collided()
					{
						Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
						IsRigid = true,
					},
					DamageSetting = new Damage()
					{
						Value = 10,
						Kind = DamageKind.Poison,
					},
				}),
				ItemKind.Speed => (IItemProxy)Owner.CreateProxy(new Item()
				{
					Kind = ItemKind.Speed,
					Body = new Collided()
					{
						Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
						IsRigid = true,
					},
					Speed = 5,
				}),
				ItemKind.AttackSpeed => (IItemProxy)Owner.CreateProxy(new Item()
				{
					Kind = ItemKind.AttackSpeed,
					Body = new Collided()
					{
						Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
						IsRigid = true,
					},
					AttackCooldown = 1000,
				}),
				_ => throw new System.InvalidOperationException(),
			};

			Items.Add(item);
		}
	}
}
