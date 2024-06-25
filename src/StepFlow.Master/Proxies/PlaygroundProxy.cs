using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy : IProxyBase<Playground>
	{
		ICollection<Material> Items { get; }

		ICollection<Material> ItemsProxy => Owner.CreateCollectionUsedProxy(Items);

		void CreateEnemy(Rectangle bounds, Rectangle vision, Strategy strategy, ItemKind releaseItem, Vector2 beginVector);
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
			var body = new Collided()
			{
				Current = { bounds },
				IsRigid = true,
			};
			var projectile = new Projectile()
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

		public void CreateEnemy(Rectangle bounds, Rectangle vision, Strategy strategy, ItemKind releaseItem, Vector2 beginVector)
		{
			var body = new Collided()
			{
				Current = { bounds },
				IsRigid = true,
			};

			body.PositionSync();

			var enemy = new Enemy()
			{
				Body = body,
				Vision = new Collided()
				{
					Current = { vision },
				},
				Cooldown = Scale.CreateByMax(10000),
				Strength = Scale.CreateByMax(100),
				Strategy = strategy,
				AttackStrategy = AttackStrategy.Bottom,
				ReleaseItem = releaseItem,
				Schedulers =
				{
					new SchedulerRunner()
					{
						Scheduler = new SchedulerVector()
						{
							Collided = body,
							Vectors =
							{
								new CourseVector()
								{
									Name = Material.SHEDULER_CONTROL_NAME,
									Value = beginVector,
								},
							},
						}
					},
				},
				Speed = 10,
			};

			enemy.Vision.PositionSync();

			Owner.GetPlaygroundItemsProxy().Add(enemy);
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
				ItemKind.Fire => new Item()
				{
					Kind = ItemKind.Fire,
					Body = new Collided()
					{
						Current = { bounds },
						IsRigid = true,
					},
					DamageSetting = new Damage()
					{
						Value = 10,
						Kind = DamageKind.Fire,
					},
				},
				ItemKind.Poison => new Item()
				{
					Kind = ItemKind.Poison,
					Body = new Collided()
					{
						Current = { bounds },
						IsRigid = true,
					},
					DamageSetting = new Damage()
					{
						Value = 10,
						Kind = DamageKind.Poison,
					},
				},
				ItemKind.Speed => new Item()
				{
					Kind = ItemKind.Speed,
					Body = new Collided()
					{
						Current = { bounds },
						IsRigid = true,
					},
					Speed = 2,
				},
				ItemKind.AttackSpeed => new Item()
				{
					Kind = ItemKind.AttackSpeed,
					Body = new Collided()
					{
						Current = { bounds },
						IsRigid = true,
					},
					AttackCooldown = 3000,
				},
				ItemKind.AddStrength => new Item()
				{
					Kind = ItemKind.AddStrength,
					Body = new Collided()
					{
						Current = { bounds },
						IsRigid = true,
					},
					AddStrength = 20,
				},
				_ => throw new System.InvalidOperationException(),
			};

			item.GetBodyRequired().PositionSync();

			Owner.GetPlaygroundItemsProxy().Add(item);
		}

		public void CreatePlace(Rectangle bounds)
		{
			var place = new Place()
			{
				Body = new Collided()
				{
					Current = { bounds },
				},
			};

			place.Body.PositionSync();

			Owner.GetPlaygroundItemsProxy().Add(place);
		}
	}
}
