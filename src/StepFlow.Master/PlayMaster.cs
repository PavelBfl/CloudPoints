using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Elements;
using StepFlow.Master.Scripts;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		public const string TAKE_STEP_CALL = "TakeStep";

		public PlayMaster()
		{
			AddExecutor(TakeStep = new EmptyExecutor(TAKE_STEP_CALL, TakeStepInner));
			AddExecutor(PlayerCharacterCreate = new PlayerCharacterCreate(this));
			AddExecutor(PlayerCharacterSetCourse = new PlayerCharacterSetCourse(this));
			AddExecutor(CreateObstruction = new CreateObstruction(this));
			AddExecutor(CreateProjectile = new CreateProjectile(this));
			AddExecutor(CreateDamageItem = new CreateDamageItem(this));
			AddExecutor(CreateSpeedItem = new CreateSpeedItem(this));
			AddExecutor(CreateEnemy = new CreateEnemy(this));
		}

		private readonly Dictionary<string, Executor> executors = new Dictionary<string, Executor>();

		private void AddExecutor(Executor executor) => executors.Add(executor.Name, executor);

		public IReadOnlyDictionary<string, Executor> Executors => executors;

		public Executor TakeStep { get; }

		public PlayerCharacterCreate PlayerCharacterCreate { get; }

		public PlayerCharacterSetCourse PlayerCharacterSetCourse { get; }

		public CreateObstruction CreateObstruction { get; }

		public CreateProjectile CreateProjectile { get; }

		public CreateDamageItem CreateDamageItem { get; }

		public CreateSpeedItem CreateSpeedItem { get; }

		public CreateEnemy CreateEnemy { get; }


		public IAxis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public long Time { get; private set; }

		public Playground Playground { get; } = new Playground();

		public IPlaygroundProxy GetPlaygroundProxy() => new PlaygroundProxy(this, Playground);

		[return: NotNullIfNotNull("value")]
		public object? CreateProxy(object? value)
		{
			return value switch
			{
				Playground instance => new PlaygroundProxy(this, instance),
				PlayerCharacter instance => new PlayerCharacterProxy(this, instance),
				Obstruction instance => new ObstructionProxy(this, instance),
				Bordered instance => new BorderedProxy(this, instance),
				Cell instance => new CellProxy(this, instance),
				SetCourse instance => new SetCourseProxy(this, instance),
				Turn instance => new TurnProxy(this, instance),
				Scale instance => new ScaleProxy(this, instance),
				Projectile instance => new ProjectileProxy(this, instance),
				Damage instance => new DamageProxy(this, instance),
				Item instance => new ItemProxy(this, instance),
				Enemy instance => new EnemyProxy(this, instance),
				Collided instance => new CollidedProxy(this, instance),
				Scheduled instance => new ScheduledProxy(this, instance),
				Core.Components.Action instance => new ActionProxy(this, instance),
				null => null,
				_ => throw new InvalidOperationException(),
			};
		}

		private void TakeStepInner()
		{
			foreach (var collision in Playground.GetMaterials()
				.Select(x => (IMaterialProxy<Material>)CreateProxy(x))
				.ToArray()
			)
			{
				collision.OnTick();
			}

			foreach (var collision in Playground.GetCollision().ToArray())
			{
				var firstMaterialProxy = (IMaterialProxy<Material>)CreateProxy(collision.Item1.Element);
				var firstCollidedProxy = (ICollidedProxy)CreateProxy(collision.Item1.Component);
				var secondMaterialProxy = (IMaterialProxy<Material>)CreateProxy(collision.Item2.Element);
				var secondCollidedProxy = (ICollidedProxy)CreateProxy(collision.Item2.Component);

				firstMaterialProxy.Collision(firstCollidedProxy, secondMaterialProxy, secondCollidedProxy);
				secondMaterialProxy.Collision(secondCollidedProxy, firstMaterialProxy, firstCollidedProxy);
			}

			foreach (var collision in Playground.GetMaterials()
				.Select(x => x.Body)
				.OfType<Collided>()
				.Select(x => (ICollidedProxy)CreateProxy(x))
				.ToArray()
			)
			{
				collision.Move();
			}

			Time++;
		}

		public void Execute(string commandName, IReadOnlyDictionary<string, object>? parameters = null)
			=> Executors[commandName].Execute(parameters);
	}
}
