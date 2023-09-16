using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Components.Custom;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public static class Components
	{
		public static class Types
		{
			public const string COLLIDED = "CollidedType";
			public const string SCALE = "ScaleType";
			public const string SCHEDULER = "SchedulerType";
			public const string DAMAGE= "DamageType";
		}

		public static class Handlers
		{
			public const string COLLISION = "CollisionHandler";
			public const string SCALE = "ScaleHandler";
			public const string COURSE = "CourseHandler";
			public const string PROJECTILE_BUILDER = "ProjectileBuilder";
			public const string REMOVE_SUBJECT = "RemoveSubject";
			public const string REMOVE_COMPONENT = "RemoveComponent";
			public const string SET_DAMAGE = "SetDamage";
		}

		public static class Names
		{
			public const string STRENGTH = "Strength";
			public const string COLLIDED = "Collided";
			public const string DAMAGE = "Damage";
			public const string MAIN_SCHEDULER = "MainScheduler";
		}
	}

	public class PlayMaster
	{
		private const string TAKE_STEP_NAME = nameof(TakeStep);
		private const string ENUMERATE_NAME = "Enumerate";

		private const string TAKE_STEP_CALL = TAKE_STEP_NAME + "()";

		public const string FIRE_DAMAGE = "Fire";
		public const string POISON_DAMAGET = "Poison";

		public PlayMaster()
		{
			InitLua();
		}

		public IAxis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public long Time { get; private set; }

		public Playground Playground { get; } = new Playground();

		public IPlaygroundProxy GetPlaygroundProxy() => CreateProxy(Playground);

		[return: NotNullIfNotNull("component")]
		internal IComponentProxy? CreateComponentProxy(IComponent? component)
		{
			if (component is IComponentProxy componentProxy)
			{
				return componentProxy;
			}
			else
			{
				return (IComponentProxy?)CreateProxy(component);
			}
		}

		[return: NotNullIfNotNull("obj")]
		internal object? CreateProxy(object? obj) => obj switch
		{
			Playground playground => CreateProxy(playground),
			Subject subject => CreateProxy(subject),
			Collided collided => CreateProxy(collided),
			Damage projectile => CreateProxy(projectile),
			Scale scale => CreateProxy(scale),
			Scheduled scheduled => CreateProxy(scheduled),
			Cell cell => CreateProxy(cell),
			Bordered bordered => CreateProxy(bordered),
			null => null,
			_ => throw new InvalidOperationException(),
		};

		[return: NotNullIfNotNull("obj")]
		internal ICellProxy? CreateProxy(Cell? obj) => obj is null ? null : new CellProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IBorderedProxy? CreateProxy(Bordered? obj) => obj is null ? null : new BorderedProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IPlaygroundProxy? CreateProxy(Playground? obj) => obj is null ? null : new PlaygroundProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal ISubjectProxy? CreateProxy(Subject? obj) => obj is null ? null : new SubjectProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal ICollidedProxy? CreateProxy(Collided? obj) => obj is null ? null : new CollidedProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IDamageProxy? CreateProxy(Damage? obj) => obj is null ? null : new DamageProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IScaleProxy? CreateProxy(Scale? obj) => obj is null ? null : new ScaleProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IScheduledProxy? CreateProxy(Scheduled? obj) => obj is null ? null : new ScheduledProxy(this, obj);

		public IComponent CreateComponent(string componentType)
		{
			if (componentType is null)
			{
				throw new ArgumentNullException(nameof(componentType));
			}

			return componentType switch
			{
				Components.Types.COLLIDED => new Collided(Playground),
				Components.Types.SCALE => new Scale(Playground),
				Components.Types.SCHEDULER => new Scheduled(Playground),
				Components.Types.DAMAGE => new Damage(Playground),

				Components.Handlers.COLLISION => new CollisionHandler(this),
				Components.Handlers.SCALE => new ScaleHandler(this),
				Components.Handlers.COURSE => new CourseHandler(this),
				Components.Handlers.PROJECTILE_BUILDER => new ProjectileBuilderHandler(this),
				Components.Handlers.REMOVE_SUBJECT => new RemoveSubjectHandler(this),
				Components.Handlers.REMOVE_COMPONENT => new RemoveComponentHandler(this),
				Components.Handlers.SET_DAMAGE => new SetDamage(this),
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep() => Execute(TAKE_STEP_CALL);

		private void CollisionHandle(ISubjectProxy main, ISubjectProxy other)
		{
			var collided = (ICollidedProxy)main.GetComponentRequired(Components.Names.COLLIDED);
			foreach (var handler in collided.Collision.Cast<ICollisionHandler>())
			{
				handler.Collision(main, other);
			}
		}

		private void TakeStepInner()
		{
			var playground = GetPlaygroundProxy();

			foreach (var collision in playground.GetCollision().ToArray())
			{
				CollisionHandle(collision.Item1, collision.Item2);
				CollisionHandle(collision.Item2, collision.Item1);
			}

			foreach (var collided in playground.Subjects
				.ToArray()
				.Select(x => x.GetComponent(Components.Names.COLLIDED))
				.OfType<ICollidedProxy>()
			)
			{
				collided.Move();
			}

			Time++;

			foreach (var scheduler in playground.Subjects
				.ToArray()
				.SelectMany(x => x.GetComponents())
				.OfType<IScheduledProxy>()
			)
			{
				scheduler.TryDequeue();
			}
		}

		private static void RegisterList<T>()
		{
			UserData.RegisterType<T>();

			UserData.RegisterType<IEnumerable<T>>();
			UserData.RegisterType<IEnumerator<T>>();
			UserData.RegisterType<IReadOnlyList<T>>();
			UserData.RegisterType<IReadOnlyCollection<T>>();
			UserData.RegisterType<IList<T>>();
			UserData.RegisterType<ICollection<T>>();
		}

		private void InitLua()
		{
			UserData.RegisterType<IEnumerator>();
			UserData.RegisterType<Rectangle>();
			UserData.RegisterType<Point>();

			RegisterList<(Subject, Subject)>();
			RegisterList<ICellProxy>();
			RegisterList<IBorderedProxy>();

			RegisterList<IComponentController>();
			RegisterList<IComponentProxy>();

			RegisterList<IPlaygroundProxy>();
			RegisterList<ISubjectProxy>();
			RegisterList<ICollidedProxy>();
			RegisterList<IDamageProxy>();
			RegisterList<IScaleProxy>();
			RegisterList<IScheduledProxy>();
		}

		public static DynValue Enumerate(ScriptExecutionContext context, CallbackArguments arguments)
		{
			var dynEnumerable = arguments[0];
			var enumerable = (IEnumerable)dynEnumerable.UserData.Object;

			return DynValue.NewTuple(
				DynValue.NewCallback(EnumerateIteration),
				dynEnumerable,
				DynValue.FromObject(context.GetScript(), enumerable.GetEnumerator())
			);

			DynValue EnumerateIteration(ScriptExecutionContext context, CallbackArguments arguments)
			{
				var dynEnumerable = arguments[0];
				var enumerable = (IEnumerable)dynEnumerable.UserData.Object;

				var dynEnumerator = arguments[1];
				var enumerator = (IEnumerator)dynEnumerator.UserData.Object;

				if (enumerator.MoveNext())
				{
					return DynValue.NewTuple(
						dynEnumerator,
						DynValue.FromObject(context.GetScript(), enumerator.Current)
					);
				}
				else
				{
					return DynValue.Nil;
				}
			}
		}

		public void Execute(string scriptText)
		{
			if (scriptText == TAKE_STEP_CALL)
			{
				TakeStepInner();
			}
			else
			{
				var script = new Script();

				script.Globals["playground"] = GetPlaygroundProxy();

				script.Globals[TAKE_STEP_NAME] = (Action)TakeStepInner;





				script.Globals.Set(ENUMERATE_NAME, DynValue.NewCallback(Enumerate));
				script.DoString(scriptText); 
			}
		}
	}
}
