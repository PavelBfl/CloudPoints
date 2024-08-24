using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core;
using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Domains.Tracks;
using StepFlow.Intersection;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Elements;
using StepFlow.Master.Proxies.Intersection;
using StepFlow.Master.Proxies.States;
using StepFlow.Master.Proxies.Tracks;
using StepFlow.Master.Scripts;
using StepFlow.TimeLine.Transactions;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		public const string TAKE_STEP_CALL = "TakeStep";

		public PlayMaster(PlaygroundDto init)
		{
			NullValidate.ThrowIfArgumentNull(init, nameof(init));

			Playground = new Playground(new Core.Context(), init);

			AddExecutor(TakeStep = new EmptyExecutor(TAKE_STEP_CALL, TakeStepInner));
			AddExecutor(PlayerCharacterCreate = new PlayerCharacterCreate(this));
			AddExecutor(PlayerCharacterSetCourse = new PlayerCharacterSetCourse(this));
			AddExecutor(CreateObstruction = new CreateObstruction(this));
			AddExecutor(PlayerCharacterCreateProjectile = new PlayerCharacterCreateProjectile(this));
			AddExecutor(CreateItem = new CreateItem(this));
			AddExecutor(CreateEnemy = new CreateEnemy(this));
			AddExecutor(CreatePlace = new CreatePlace(this));
		}

		#region Executors
		private readonly Dictionary<string, Executor> executors = new Dictionary<string, Executor>();

		private void AddExecutor(Executor executor) => executors.Add(executor.Name, executor);

		public IReadOnlyDictionary<string, Executor> Executors => executors;

		public Executor TakeStep { get; }

		public PlayerCharacterCreate PlayerCharacterCreate { get; }

		public PlayerCharacterSetCourse PlayerCharacterSetCourse { get; }

		public CreateObstruction CreateObstruction { get; }

		public PlayerCharacterCreateProjectile PlayerCharacterCreateProjectile { get; }

		public CreateItem CreateItem { get; }

		public CreateEnemy CreateEnemy { get; }

		public CreatePlace CreatePlace { get; }

		public void Execute(string commandName, IReadOnlyDictionary<string, object>? parameters = null)
			=> Executors[commandName].Execute(parameters);
		#endregion

		public TransactionAxis TimeAxis { get; } = new TransactionAxis();

		public WormholeDto? Wormhole { get; set; }

		public PlayerCharacterDto? PlayerCharacterPop()
		{
			if (Playground.Items.OfType<PlayerCharacter>().SingleOrDefault() is { } playerCharacter)
			{
				var itemsProxy = GetPlaygroundItemsProxy();
				itemsProxy.Remove(playerCharacter);
				return (PlayerCharacterDto)playerCharacter.ToDto();
			}
			else
			{
				return null;
			}
		}

		public void PlayerCharacterPush(PlayerCharacterDto playerDto)
		{
			if (playerDto is null)
			{
				throw new ArgumentNullException(nameof(playerDto));
			}

			if (Playground.Items.OfType<PlayerCharacter>().SingleOrDefault() is { })
			{
				throw new InvalidOperationException();
			}

			var player = new PlayerCharacter(Playground.Context, playerDto);
			var itemsProxy = GetPlaygroundItemsProxy();
			itemsProxy.Add(player);
		}

		public Playground Playground { get; }

		[return: NotNullIfNotNull("rectangles")]
		public ShapeBase? CreateShape(IEnumerable<Rectangle>? rectangles)
			=> rectangles is null ? null : ShapeBase.Create(Playground.Context.IntersectionContext, rectangles);

		public ShapeBase CreateShape(Rectangle rectangle) => ShapeBase.Create(Playground.Context.IntersectionContext, rectangle);

		[return: NotNullIfNotNull("value")]
		public object? CreateProxy(object? value)
		{
			return value switch
			{
				Playground instance => new PlaygroundProxy(this, instance),
				PlayerCharacter instance => new PlayerCharacterProxy(this, instance),
				Obstruction instance => new ObstructionProxy(this, instance),
				Projectile instance => new ProjectileProxy(this, instance),
				Item instance => new ItemProxy(this, instance),
				Enemy instance => new EnemyProxy(this, instance),
				Collided instance => new CollidedProxy(this, instance),
				Intersection.Context instance => new ContextProxy(this, instance),
				ShapeCell instance => new ShapeCellProxy(this, instance),
				ShapeContainer instance => new ShapeContainerProxy(this, instance),
				Place instance => new PlaceProxy(this, instance),
				Track instance => new TrackProxy(this, instance),
				TrackBuilder instance => new TrackBuilderProxy(this, instance),
				TrackUnit instance => new TrackUnitProxy(this, instance),
				State instance => new StateProxy(this, instance),
				Wormhole instance => new WormholeProxy(this, instance),
				null => null,
				_ => throw new InvalidOperationException(),
			};
		}

		public IList<T> CreateListProxy<T>(IList<T> target) => new ListProxy<T, IList<T>>(this, target);

		public ICollection<T> CreateCollectionProxy<T>(ICollection<T> target) => new CollectionProxy<T, ICollection<T>>(this, target);

		public IPlaygroundProxy GetPlaygroundProxy() => (IPlaygroundProxy)CreateProxy(Playground);

		public ICollection<Material> GetPlaygroundItemsProxy() => CreateCollectionProxy(Playground.Items);

		private void TakeStepInner()
		{
			var materials = Playground.Items
				.Select(x => (IMaterialProxy<Material>)CreateProxy(x))
				.ToArray();

			foreach (var collision in materials)
			{
				collision.OnTick();
			}

			var collisions = Playground.Context.IntersectionContext.GetCollisions();
			foreach (var collision in collisions)
			{
				if (collision.Left.Attached is { } leftAttached && collision.Right.Attached is { } rightAttached)
				{
					var leftCollided = (CollidedAttached)leftAttached;
					var leftMaterial = (IMaterialProxy<Material>)CreateProxy(leftCollided.Collided.GetElementRequired());

					var rightCollided = (CollidedAttached)rightAttached;
					var rightMaterial = (IMaterialProxy<Material>)CreateProxy(rightCollided.Collided.GetElementRequired());

					leftMaterial.Collision(leftCollided, rightMaterial.Target, rightCollided);
					//rightMaterial.Collision(rightCollided, leftMaterial.Target, leftCollided);
				}
			}

			foreach (var collision in Playground.Items
				.Select(x => x.Body)
				.Select(CreateProxy)
				.Cast<ICollidedProxy>()
			)
			{
				collision.Move();
			}
		}

		public void CreateProjectile(Point center, int radius, Vector2 course, Damage damage, int duration, Subject? creator, ReusableKind reusable)
		{
			var projectile = new Projectile(Playground.Context)
			{
				Name = "Projectile",
				Body = new Collided(Playground.Context)
				{
					Current = CreateShape(RectangleExtensions.Create(center, radius)),
				},
				Damage = damage,
				Reusable = reusable,
				Speed = 100,
				Course = course,
				States =
				{
					new State(Playground.Context)
					{
						Kind = StateKind.Remove,
						TotalCooldown = duration,
					},
				},
				Track = new TrackBuilder(Playground.Context)
				{
					Cooldown = Scale.CreateByMax(TimeTick.FromSeconds(0.05f)),
					Change = new TrackChange(Playground.Context)
					{
						Size = new Vector2(-0.003f),
						Position = course * 0.002f,
						View = TrackView.None,
					},
				},
			};

			if (creator is { })
			{
				projectile.Immunity.Add(creator);
			}

			GetPlaygroundItemsProxy().Add(projectile);
		}

		#region Accessors

		private Dictionary<(Type, string), object> AccessorsCache { get; } = new Dictionary<(Type, string), object>();

		public IValueAccessor<TTarget, TValue> GetAccessor<TTarget, TValue>(string propertyName)
		{
			var key = (typeof(TTarget), propertyName);
			if (!AccessorsCache.TryGetValue(key, out var accessor))
			{
				var propertyInfo = typeof(TTarget).GetProperty(propertyName);

				accessor = AccessorsExtensions.CreatePropertyAccessor<TTarget, TValue>(propertyInfo);

				AccessorsCache.Add(key, accessor);
			}

			return (IValueAccessor<TTarget, TValue>)accessor;
		}

		#endregion
	}
}
