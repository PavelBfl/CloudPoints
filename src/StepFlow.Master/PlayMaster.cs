using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Components;
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
			public const string PROJECTILE_SETTINGS = "ProjectileSettingsType";
			public const string HANDLER = "Handler";
			public const string SET_COURSE = "CourseHandler";
			public const string SENTRY_GUN = "SentryGun";
			public const string SYSTEM = "System";
			public const string STATE = "State";
		}

		public static class Names
		{
			public const string STRENGTH = "Strength";
			public const string COLLIDED = "Collided";
			public const string DAMAGE = "Damage";
			public const string MAIN_SCHEDULER = "MainScheduler";
			public const string PROJECTILE_SETTINGS = "ProjectileSettings";
			public const string PROJECTILE_SETTINGS_SET = "ProjectileSettingsSet";
			public const string VISION = "Vision";
			public const string STATE = "State";
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

		public IPlaygroundProxy GetPlaygroundProxy() => (IPlaygroundProxy)CreateProxy(Playground);

		[return: NotNullIfNotNull("obj")]
		internal object? CreateProxy(object? obj)
		{
			if (obj is null)
			{
				return null;
			}

			foreach (var (key, constructor) in Proxies)
			{
				if (key.IsAssignableFrom(obj.GetType()))
				{
					return constructor.Invoke(new object[] { this, obj });
				}
			}

			throw new InvalidOperationException();
		}

		private void RegisterProxy(Type target, Type proxy)
		{
			var constructorInfo = proxy.GetConstructor(new Type[] { typeof(PlayMaster), target });
			Proxies.Add(target, constructorInfo);
		}

		private Dictionary<Type, ConstructorInfo> Proxies { get; } = new Dictionary<Type, ConstructorInfo>();


		private void RegisterComponent(Type proxy)
		{
			var attribute = proxy.GetCustomAttribute<ComponentProxyAttribute>();

			var name = attribute.Name ?? attribute.Target.Name;
			RegisterComponent(name, attribute.Target, attribute.Proxy);
		}

		private void RegisterComponent(string name, Type target, Type proxy)
		{
			RegisterProxy(target, proxy);

			var constructorInfo = target.GetConstructor(new Type[] { typeof(Playground) });
			Components.Add(name, constructorInfo);
		}

		private Dictionary<string, ConstructorInfo> Components { get; } = new Dictionary<string, ConstructorInfo>();

		private static IEnumerable<KeyValuePair<string, Action<IHandlerProxy, IComponentProxy>>> GetHandlers(Type container)
		{
			if (container is null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			var methods = container.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
			foreach (var method in methods)
			{
				var handler = (Action<IHandlerProxy, IComponentProxy>)method.CreateDelegate(typeof(Action<IHandlerProxy, IComponentProxy>));
				yield return KeyValuePair.Create(method.Name, handler);
			}
		}

		public IReadOnlyDictionary<string, Action<IHandlerProxy, IComponentProxy>> Handlers { get; } = new Dictionary<string, Action<IHandlerProxy, IComponentProxy>>(GetHandlers(typeof(Handlers)));

		public IComponent CreateComponent(string componentType)
		{
			if (componentType is null)
			{
				throw new ArgumentNullException(nameof(componentType));
			}

			return (IComponent)Components[componentType].Invoke(new object[] { Playground });
		}

		public void TakeStep() => Execute(TAKE_STEP_CALL);

		private void CollisionHandle(ICollidedProxy main, ICollidedProxy other)
		{
			foreach (var handler in main.Collision)
			{
				handler.Handle(other);
			}
		}

		private void TakeStepInner()
		{
			var playground = GetPlaygroundProxy();

			foreach (var system in playground.Subjects.SelectMany(x => x.GetComponents()).OfType<ISystemProxy>())
			{
				foreach (var handler in system.OnFrame)
				{
					handler.Handle(null);
				}
			}

			foreach (var collision in playground.GetCollision().ToArray())
			{
				CollisionHandle(collision.Item1, collision.Item2);
				CollisionHandle(collision.Item2, collision.Item1);
			}

			foreach (var collided in playground.Subjects
				.ToArray()
				.Select(x => x.GetComponent(Master.Components.Names.COLLIDED))
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

			RegisterList<(ICollidedProxy, ICollidedProxy)>();
			RegisterList<ICellProxy>();
			RegisterList<IBorderedProxy>();

			RegisterList<IContainerProxy>();
			RegisterList<IComponentProxy>();

			RegisterList<IPlaygroundProxy>();
			RegisterList<ISubjectProxy>();
			RegisterList<ICollidedProxy>();
			RegisterList<IDamageProxy>();
			RegisterList<IScaleProxy>();
			RegisterList<IScheduledProxy>();
			RegisterList<IProjectileSettingsProxy>();

			RegisterProxy(typeof(Playground), typeof(PlaygroundProxy));
			RegisterProxy(typeof(Subject), typeof(SubjectProxy));
			RegisterProxy(typeof(Cell), typeof(CellProxy));
			RegisterProxy(typeof(Bordered), typeof(BorderedProxy));

			RegisterComponent(typeof(ICollidedProxy));
			RegisterComponent(typeof(IDamageProxy));
			RegisterComponent(typeof(IScaleProxy));
			RegisterComponent(typeof(IScheduledProxy));
			RegisterComponent(typeof(IProjectileSettingsProxy));
			RegisterComponent(typeof(ISetCourseProxy));
			RegisterComponent(typeof(IHandlerProxy));
			RegisterComponent(typeof(ISentryGunProxy));
			RegisterComponent(typeof(ISystemProxy));
			RegisterComponent(typeof(IStateProxy));
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
