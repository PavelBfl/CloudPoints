using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Components;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		private const string TAKE_STEP_NAME = nameof(TakeStep);
		private const string ENUMERATE_NAME = "Enumerate";

		private const string TAKE_STEP_CALL = TAKE_STEP_NAME + "()";

		public PlayMaster()
		{
			InitLua();
		}

		public IAxis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public long Time { get; private set; }

		public Playground Playground { get; } = new Playground();

		[return: NotNullIfNotNull("obj")]
		internal object? CreateProxy(object? obj) => obj switch
		{
			Playground playground => CreateProxy(playground),
			Subject subject => CreateProxy(subject),
			Collided collided => CreateProxy(collided),
			Projectile projectile => CreateProxy(projectile),
			Scale scale => CreateProxy(scale),
			Scheduled scheduled => CreateProxy(scheduled),
			null => null,
			_ => throw new InvalidOperationException(),
		};

		[return: NotNullIfNotNull("obj")]
		internal IPlaygroundProxy? CreateProxy(Playground? obj) => obj is null ? null : new PlaygroundProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal ISubjectProxy? CreateProxy(Subject? obj) => obj is null ? null : new SubjectProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal ICollidedProxy? CreateProxy(Collided? obj) => obj is null ? null : new CollidedProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IProjectileProxy? CreateProxy(Projectile? obj) => obj is null ? null : new ProjectileProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IScaleProxy? CreateProxy(Scale? obj) => obj is null ? null : new ScaleProxy(this, obj);

		[return: NotNullIfNotNull("obj")]
		internal IScheduledProxy? CreateProxy(Scheduled? obj) => obj is null ? null : new ScheduledProxy(this, obj);

		public IComponent CreateComponent(string componentName)
		{
			if (componentName is null)
			{
				throw new ArgumentNullException(nameof(componentName));
			}

			return componentName switch
			{
				Playground.COLLIDED_NAME => new Collided(),
				Playground.STRENGTH_NAME => new Scale(),
				Playground.SCHEDULER_NAME => new Scheduled(),
				Playground.PROJECTILE_NAME => new Projectile(),
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep() => Execute(TAKE_STEP_CALL);

		private void CollisionHandle(ISubjectProxy strength, ISubjectProxy damage)
		{
			if (
				strength.GetComponent(Playground.STRENGTH_NAME) is IScaleProxy scale &&
				damage.GetComponent(Playground.PROJECTILE_NAME) is IProjectileProxy projectile
			)
			{
				scale.Add(-projectile.Damage);
			}
		}

		private void TakeStepInner()
		{
			var removed = new List<ISubjectProxy>();
			foreach (var collision in Playground.GetCollision())
			{
				var item1 = (ISubjectProxy)CreateProxy(collision.Item1);
				var item2 = (ISubjectProxy)CreateProxy(collision.Item2);

				CollisionHandle(item1, item2);
				CollisionHandle(item2, item1);

				((ICollidedProxy?)item1.GetComponent(Playground.COLLIDED_NAME))?.Break();
				((ICollidedProxy?)item2.GetComponent(Playground.COLLIDED_NAME))?.Break();

				if (item1.GetComponent(Playground.PROJECTILE_NAME) is { })
				{
					removed.Add(item1);
				}

				if (item2.GetComponent(Playground.PROJECTILE_NAME) is { })
				{
					removed.Add(item2);
				}
			}

			var playgroundProxy = (PlaygroundProxy)CreateProxy(Playground);
			foreach (var subject in removed)
			{
				playgroundProxy.Subjects.Remove(subject);
			}

			foreach (var subject in Playground.Subjects
				.Select(x => (SubjectProxy)CreateProxy(x))
				.ToArray()
			)
			{
				if (subject.GetComponent(Playground.COLLIDED_NAME) is ICollidedProxy collided)
				{
					collided.Move();
				}
			}

			Time++;

			foreach (var scheduler in Playground.Subjects
				.ToArray()
				.Select(x => x.Components[Playground.SCHEDULER_NAME])
				.OfType<Scheduled>()
				.Where(x => x.Queue.Any())
			)
			{
				var schedulerProxy = (ScheduledProxy)CreateProxy(scheduler);
				schedulerProxy.TryDequeue();
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

			RegisterList<IComponentController>();
			RegisterList<IContainerProxy>();
			RegisterList<IComponentProxy>();

			RegisterList<IPlaygroundProxy>();
			RegisterList<ISubjectProxy>();
			RegisterList<ICollidedProxy>();
			RegisterList<IProjectileProxy>();
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

				script.Globals["playground"] = Playground;

				script.Globals[TAKE_STEP_NAME] = (Action)TakeStepInner;
				script.Globals.Set(ENUMERATE_NAME, DynValue.NewCallback(Enumerate));
				script.DoString(scriptText); 
			}
		}
	}
}
