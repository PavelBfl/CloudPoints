using System.Collections.Generic;
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
			PlayerCharacter = (IPlayerCharacterProxy)Owner.CreateProxy(new PlayerCharacter(Target.Context)
			{
				Strength = new Scale(Target.Context)
				{
					Max = 1,
					Value = 1,
				},
				Cooldown = new Scale(Target.Context)
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
			var barrier = (IObstructionProxy)Owner.CreateProxy(new Obstruction(Target.Context));
			barrier.Current = CreateCell(bounds);

			if (strength is { })
			{
				barrier.Strength = (IScaleProxy)Owner.CreateProxy(new Scale(Target.Context)
				{
					Value = strength.Value,
					Max = strength.Value,
				});
			}

			Obstructions.Add(barrier);
		}

		public void CreateProjectile(Rectangle bounds, int value, DamageKind kind)
		{
			var projectile = (IProjectileProxy)Owner.CreateProxy(new Projectile(Target.Context)
			{
				Current = new Cell()
				{
					Border = bounds,
				},
				Damage = new Damage(Target.Context)
				{
					Value = value,
					Kind = kind,
				},
			});

			Projectiles.Add(projectile);
		}

		public void CreateItem(Rectangle bounds, int value, DamageKind kind)
		{
			var item = (IItemProxy)Owner.CreateProxy(new Item(Target.Context)
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
	}
}
