using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy : IProxyBase<Playground>
	{
		ICollection<Material> Items { get; }

		ICollection<Material> ItemsProxy => Owner.CreateCollectionProxy(Items);

		void CreateItem(Point position, ItemKind kind);

		void CreatePlace(Rectangle bounds);
	}

	internal sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public ICollection<Material> Items => Target.Items;

		public void CreateProjectile(Rectangle bounds, int value, DamageKind kind)
		{
			var body = new Collided(Owner.Playground.Context)
			{
				Current = Owner.CreateShape(bounds),
				IsRigid = true,
			};
			var projectile = new Projectile(Owner.Playground.Context)
			{
				Body = body,
				Damage = new Damage()
				{
					Value = value,
					Kind = kind,
				},
				Speed = 5,
			};

			Owner.GetPlaygroundItemsProxy().Add(projectile);
		}

		public void CreateItem(Point position, ItemKind kind)
		{
			if (kind == ItemKind.None)
			{
				return;
			}

			var bounds = new Rectangle(position, new Size(Playground.CELL_SIZE_DEFAULT, Playground.CELL_SIZE_DEFAULT));

			var item = kind switch
			{
				ItemKind.Fire => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.Fire,
					Body = new Collided(Owner.Playground.Context)
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
					DamageSetting = new Damage()
					{
						Value = 10,
						Kind = DamageKind.Fire,
					},
				},
				ItemKind.Poison => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.Poison,
					Body = new Collided(Owner.Playground.Context)
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
					DamageSetting = new Damage()
					{
						Value = 10,
						Kind = DamageKind.Poison,
					},
				},
				ItemKind.Speed => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.Speed,
					Body = new Collided(Owner.Playground.Context)
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
					Speed = 2,
				},
				ItemKind.AttackSpeed => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.AttackSpeed,
					Body = new Collided(Owner.Playground.Context)
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
					AttackCooldown = 3000,
				},
				ItemKind.AddStrength => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.AddStrength,
					Body = new Collided(Owner.Playground.Context)
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
					AddStrength = 20,
				},
				_ => throw new System.InvalidOperationException(),
			};

			Owner.GetPlaygroundItemsProxy().Add(item);
		}

		public void CreatePlace(Rectangle bounds)
		{
			var place = new Place(Owner.Playground.Context)
			{
				Body = new Collided(Owner.Playground.Context)
				{
					Current = Owner.CreateShape(bounds),
				},
			};

			Owner.GetPlaygroundItemsProxy().Add(place);
		}
	}
}
