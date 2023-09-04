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
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		public PlayMaster()
		{
			InitLua();
		}

		public IAxis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public long Time { get; private set; }

		public Playground Playground { get; } = new Playground();

		private Dictionary<Type, IProxyFactory> Proxies { get; } = new Dictionary<Type, IProxyFactory>();

		private void RegisterProxyType<TProxy, TTarget>(Func<TTarget, TProxy> wrapDelegate)
			where TProxy : class
			where TTarget : class
		{
			var proxyFactory = new DelegateProxyFactory<TProxy, TTarget>(wrapDelegate);
			UserData.RegisterProxyType(proxyFactory);
			Proxies.Add(typeof(TTarget), proxyFactory);
		}

		[return: NotNullIfNotNull("obj")]
		internal object? CreateProxy(object? obj) => obj is null ? null : Proxies[obj.GetType()].CreateProxyObject(obj);

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
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep()
		{
			Execute(@"
			collision = playground.GetCollision();

			for _, collisionUnit in enumerate(collision) do
				CollidedHandle(collisionUnit.Item1, collisionUnit.Item2)
			end

			for _, subject in enumerate(playground.Subjects) do
				collided = subject.GetComponent(""Collided"")
				collided.Move()
			end
			");

			Time++;

			foreach (var scheduler in Playground.Subjects
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

			UserData.RegisterType<(Subject, Subject)>();
			RegisterList<(Subject, Subject)>();

			RegisterProxyType<PlaygroundProxy, Playground>(x => new PlaygroundProxy(this, x));

			RegisterProxyType<SubjectProxy<Subject>, Subject>(x => new SubjectProxy<Subject>(this, x));
			RegisterList<Subject>();

			RegisterProxyType<ListProxy<Turn, List<Turn>>, List<Turn>>(x => new ListProxy<Turn, List<Turn>>(this, x));

			RegisterProxyType<SubjectsCollectionProxy, IList<Subject>>(x => new SubjectsCollectionProxy(this, x));
			RegisterProxyType<BorderedProxy, Bordered>(x => new BorderedProxy(this, x));
			RegisterProxyType<CellProxy, Cell>(x => new CellProxy(this, x));

			RegisterProxyType<ScaleProxy, Scale>(x => new ScaleProxy(this, x));
			RegisterProxyType<CollidedProxy, Collided>(x => new CollidedProxy(this, x));
			RegisterProxyType<ScheduledProxy, Scheduled>(x => new ScheduledProxy(this, x));
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

		public DynValue CollidedHandle(ScriptExecutionContext context, CallbackArguments arguments)
		{
			var dynFirst = arguments[0];
			var firstProxy = (SubjectProxy<Subject>)CreateProxy(dynFirst.UserData.Object);

			var dynSecond = arguments[1];
			var secondProxy = (SubjectProxy<Subject>)CreateProxy(dynSecond.UserData.Object);

			var firstCollided = (CollidedProxy)CreateProxy(firstProxy.GetComponent(Playground.COLLIDED_NAME));
			var secondCollided = (CollidedProxy)CreateProxy(secondProxy.GetComponent(Playground.COLLIDED_NAME));

			if (firstProxy.GetComponent(Playground.STRENGTH_NAME) is { } firstStrength)
			{
				var firstStrengthProxy = (ScaleProxy)CreateProxy(firstStrength);
				firstStrengthProxy.Add(-secondCollided.Damage);
				firstCollided.Breck();
			}

			if (secondProxy.GetComponent(Playground.STRENGTH_NAME) is { } secondStrength)
			{
				var secondStrengthProxy = (ScaleProxy)CreateProxy(secondStrength);
				secondStrengthProxy.Add(-firstCollided.Damage);
				secondCollided.Breck();
			}

			return DynValue.Nil;
		}

		public void Execute(string scriptText)
		{
			var script = new Script();

			script.Globals["playground"] = Playground;
			script.Globals.Set("enumerate", DynValue.NewCallback(Enumerate));
			script.Globals.Set(nameof(CollidedHandle), DynValue.NewCallback(CollidedHandle));
			script.Globals["debug"] = (Action<object>)Debug;
			script.DoString(scriptText);
		}

		private void Debug(object obj)
		{
			
		}
	}
}
