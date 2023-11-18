﻿using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	internal sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public IPlayerCharacterProxy? PlayerCharacter { get => (IPlayerCharacterProxy?)Owner.CreateProxy(Target.PlayerCharacter); set => SetValue(x => x.PlayerCharacter, ((IProxyBase<PlayerCharacter>?)value)?.Target); }

		public IList<IObstructionProxy> Obstructions => new ListItemsProxy<Obstruction, IList<Obstruction>, IObstructionProxy>(Owner, Target.Obstructions);

		public IList<IProjectileProxy> Projectiles => new ListItemsProxy<Projectile, IList<Projectile>, IProjectileProxy>(Owner, Target.Projectiles);

		public IList<IItemProxy> Items => CreateListItemProxy<Item, IItemProxy>(Target.Items);

		public IList<IEnemyProxy> Enemies => CreateListItemProxy<Enemy, IEnemyProxy>(Target.Enemies);

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

		public IBorderedProxy CreateBordered() => (IBorderedProxy)Owner.CreateProxy(new Bordered());

		public ICellProxy CreateCell(Rectangle border)
		{
			return (ICellProxy)Owner.CreateProxy(new Cell()
			{
				Border = border,
			});
		}

		public void CreatePlayerCharacter(Rectangle bounds, int strength)
		{
			PlayerCharacter = (IPlayerCharacterProxy)Owner.CreateProxy(new PlayerCharacter()
			{
				Strength = new Scale()
				{
					Max = 1,
					Value = 1,
				},
				Cooldown = new Scale()
				{
					Max = 100,
					Value = 100,
				},
			});
			PlayerCharacter.Current = CreateCell(bounds);
			PlayerCharacter.Strength.Value = strength;
			PlayerCharacter.Strength.Max = strength;
		}

		public void CreateObstruction(Rectangle bounds, int? strength)
		{
			var barrier = (IObstructionProxy)Owner.CreateProxy(new Obstruction());
			barrier.Current = CreateCell(bounds);

			if (strength is { })
			{
				barrier.Strength = (IScaleProxy)Owner.CreateProxy(new Scale()
				{
					Value = strength.Value,
					Max = strength.Value,
				});
			}

			Obstructions.Add(barrier);
		}

		public void CreateProjectile(Rectangle bounds, int value, DamageKind kind)
		{
			var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile()
			{
				Current = new Cell()
				{
					Border = bounds,
				},
				Damage = new Damage()
				{
					Value = value,
					Kind = kind,
				},
			});

			Projectiles.Add(projectile);
		}

		public void CreateItem(Rectangle bounds, int value, DamageKind kind)
		{
			var item = (IItemProxy)Owner.CreateProxy(new Item()
			{
				Current = new Cell()
				{
					Border = bounds,
				},
				Value = value,
				Kind = kind,
			});

			Items.Add(item);
		}

		public void CreateEnemy(Rectangle bounds, Rectangle vision)
		{
			var enemy = (IEnemyProxy)Owner.CreateProxy(new Enemy()
			{
				Current = new Cell()
				{
					Border = bounds,
				},
				Vision = new Collided()
				{
					Current = new Cell()
					{
						Border = vision,
					},
				},
				Cooldown = new Scale()
				{
					Value = 100,
					Max = 100,
				}
			});

			Enemies.Add(enemy);
		}
	}
}
