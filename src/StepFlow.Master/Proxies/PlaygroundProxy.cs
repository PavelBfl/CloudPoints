using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Elements;
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
			var projectile = new Projectile(Owner.Playground.Context)
			{
				Damage = new Damage()
				{
					Value = value,
					Kind = kind,
				},
				Speed = 5,
			};
			projectile.Body.Current = Owner.CreateShape(bounds);
			projectile.Body.IsRigid = true;

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
					Body =
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
				},
				ItemKind.Poison => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.Poison,
					Body =
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
				},
				ItemKind.Speed => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.Speed,
					Body =
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
					Speed = 2,
				},
				ItemKind.AttackSpeed => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.AttackSpeed,
					Body =
					{
						Current = Owner.CreateShape(bounds),
						IsRigid = true,
					},
					AttackCooldown = 3000,
				},
				ItemKind.AddStrength => new Item(Owner.Playground.Context)
				{
					Kind = ItemKind.AddStrength,
					Body =
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
				Body =
				{
					Current = Owner.CreateShape(bounds),
				},
			};

			Owner.GetPlaygroundItemsProxy().Add(place);
		}
	}
}
