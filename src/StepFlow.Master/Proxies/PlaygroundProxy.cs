using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Elements;
using StepFlow.Master.Proxies.Intersection;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy : IProxyBase<Playground>
	{
		PlayerCharacter? PlayerCharacter { get; set; }
		IList<Obstruction> Obstructions { get; }
		IList<Projectile> Projectiles { get; }
		IList<Item> Items { get; }
		IList<Enemy> Enemies { get; }
		IContextProxy IntersectionContext { get; }

		void CreateObstruction(Rectangle bounds, int? strength);
		void CreatePlayerCharacter(Rectangle bounds, int strength);
		void CreateEnemy(Rectangle bounds, Rectangle vision, ItemKind releaseItem);
		void CreateItem(Point position, ItemKind kind);
		void CreatePlace(Rectangle bounds);
	}

	internal sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public IContextProxy IntersectionContext => (IContextProxy)Owner.CreateProxy(Target.IntersectionContext);

		public PlayerCharacter? PlayerCharacter { get => Target.PlayerCharacter; set => SetValue(value); }

		public IList<Obstruction> Obstructions => Target.Obstructions;

		public IList<Projectile> Projectiles => Target.Projectiles;

		public IList<Item> Items => Target.Items;

		public IList<Enemy> Enemies => Target.Enemies;

		public void CreatePlayerCharacter(Rectangle bounds, int strength)
		{
			var body = new Collided()
			{
				Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
				IsRigid = true,
			};

			PlayerCharacter = new PlayerCharacter()
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
				Body = body,
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
								},
							},
						}
					},
				},
				Speed = 10,
			};
		}

		public void CreateObstruction(Rectangle bounds, int? strength)
		{
			var barrier = new Obstruction()
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
			};

			var obstructionsProxy = CreateListProxy(Obstructions);
			obstructionsProxy.Add(barrier);
		}

		public void CreateProjectile(Rectangle bounds, int value, DamageKind kind)
		{
			var body = new Collided()
			{
				Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
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

			var projectilesProxy = CreateListProxy(Projectiles);
			projectilesProxy.Add(projectile);
		}

		public void CreateEnemy(Rectangle bounds, Rectangle vision, ItemKind releaseItem)
		{
			var body = new Collided()
			{
				Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
				IsRigid = true,
			};
			var enemy = new Enemy()
			{
				Body = body,
				Vision = new Collided()
				{
					Current = new ShapeCell(Owner.Playground.IntersectionContext, vision),
				},
				Cooldown = new Scale()
				{
					Value = 10000,
					Max = 10000,
				},
				Strength = new Scale()
				{
					Value = 100,
					Max = 100,
				},
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
									Value = new Vector2(1, 0),
								},
							},
						}
					},
				},
				Speed = 10,
			};

			var enemiesProxy = CreateListProxy(Enemies);
			enemiesProxy.Add(enemy);
		}

		public void CreateItem(Point position, ItemKind kind)
		{
			if (kind == ItemKind.None)
			{
				return;
			}

			const int RADIUS = 7;

			var bounds = RectangleExtensions.Create(position, RADIUS);

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
					Speed = 10,
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
				ItemKind.AddStrength => (IItemProxy)Owner.CreateProxy(new Item()
				{
					Kind = ItemKind.AddStrength,
					Body = new Collided()
					{
						Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
						IsRigid = true,
					},
					AddStrength = 20,
				}),
				_ => throw new System.InvalidOperationException(),
			};

			var itemsProxy = CreateListProxy(Items);
			itemsProxy.Add(item.Target);
		}

		public void CreatePlace(Rectangle bounds)
		{
			var place = new Place()
			{
				Body = new Collided()
				{
					Current = new ShapeCell(Owner.Playground.IntersectionContext, bounds),
				},
			};

			var placesProxy = CreateListProxy(Owner.Playground.Places);
			placesProxy.Add(place);
		}
	}
}
