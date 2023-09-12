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

		public IPlaygroundProxy GetPlaygroundProxy() => CreateProxy(Playground);

		[return: NotNullIfNotNull("obj")]
		internal object? CreateProxy(object? obj) => obj switch
		{
			Playground playground => CreateProxy(playground),
			Subject subject => CreateProxy(subject),
			Collided collided => CreateProxy(collided),
			CollisionDamage projectile => CreateProxy(projectile),
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
		internal ICollisionDamageProxy? CreateProxy(CollisionDamage? obj) => obj is null ? null : new CollisionDamageProxy(this, obj);

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
				Playground.COLLISION_DAMAGE_NAME => new CollisionDamage(),
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep() => Execute(TAKE_STEP_CALL);

		private void TakeStepInner()
		{
			var playground = GetPlaygroundProxy();

			foreach (var collision in playground.GetCollision().ToArray())
			{
				((ICollidedProxy?)collision.Item1.GetComponent(Playground.COLLIDED_NAME))?.CollidedHandle(collision.Item2);
				((ICollidedProxy?)collision.Item2.GetComponent(Playground.COLLIDED_NAME))?.CollidedHandle(collision.Item1);
			}

			foreach (var collided in playground.Subjects
				.ToArray()
				.Select(x => x.GetComponent(Playground.COLLIDED_NAME))
				.OfType<ICollidedProxy>()
			)
			{
				collided.Move();
			}

			Time++;

			foreach (var scheduler in playground.Subjects
				.ToArray()
				.Select(x => x.GetComponent(Playground.SCHEDULER_NAME))
				.OfType<IScheduledProxy>()
				.Where(x => !x.IsEmpty)
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
			RegisterList<ICollisionDamageProxy>();
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
