using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy : IProxyBase<Playground>
	{
		ICollection<Material> Items { get; }

		ICollection<Material> ItemsProxy => Owner.CreateCollectionUsedProxy(Items);

		void CreateObstruction(IEnumerable<Rectangle> bounds, int? strength, ObstructionKind kind);
		void CreatePlayerCharacter(Rectangle bounds, int strength);
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

		public void CreatePlayerCharacter(Rectangle bounds, int strength)
		{
			var body = new Collided()
			{
				Current = { bounds },
				IsRigid = true,
			};

			var playerCharacter = new PlayerCharacter()
			{
				Name = "Player",
				Strength = new Scale()
				{
					Max = strength,
					Value = strength,
				},
				Cooldown = new Scale()
				{
					Max = TimeTick.FromFrames(10),
					Value = TimeTick.FromFrames(10),
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

			Owner.GetPlaygroundItemsProxy().Add(playerCharacter);
		}

		public void CreateObstruction(IEnumerable<Rectangle> bounds, int? strength, ObstructionKind kind)
		{
			var barrier = new Obstruction()
			{
				Name = "Obstruction",
				Kind = kind,
				Body = new Collided()
				{
					Current = { bounds },
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

			Owner.GetPlaygroundItemsProxy().Add(barrier);
		}

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
			var enemy = new Enemy()
			{
				Body = body,
				Vision = new Collided()
				{
					Current = { vision },
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
				Strategy = strategy,
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

			Owner.GetPlaygroundItemsProxy().Add(enemy);
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
						Current = { bounds },
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
						Current = { bounds },
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
						Current = { bounds },
						IsRigid = true,
					},
					Speed = 10,
				}),
				ItemKind.AttackSpeed => (IItemProxy)Owner.CreateProxy(new Item()
				{
					Kind = ItemKind.AttackSpeed,
					Body = new Collided()
					{
						Current = { bounds },
						IsRigid = true,
					},
					AttackCooldown = 1000,
				}),
				ItemKind.AddStrength => (IItemProxy)Owner.CreateProxy(new Item()
				{
					Kind = ItemKind.AddStrength,
					Body = new Collided()
					{
						Current = { bounds },
						IsRigid = true,
					},
					AddStrength = 20,
				}),
				_ => throw new System.InvalidOperationException(),
			};

			Owner.GetPlaygroundItemsProxy().Add(item.Target);
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

			Owner.GetPlaygroundItemsProxy().Add(place);
		}
	}
}
