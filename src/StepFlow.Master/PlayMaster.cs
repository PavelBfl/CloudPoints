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
		private const string TAKE_STEP_NAME = nameof(TakeStep);
		private const string ENUMERATE_NAME = "Enumerate";

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
				Playground.PROJECTILE_NAME => new Projectile(),
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep() => Execute(TAKE_STEP_NAME + "()");

		private void CollisionHandle(Subject strength, Subject damage)
		{
			if (
				strength.Components[Playground.STRENGTH_NAME] is Scale scale &&
				damage.Components[Playground.PROJECTILE_NAME] is Projectile projectile
			)
			{
				var scaleProxy = (ScaleProxy)CreateProxy(scale);
				var projectileProxy = (ProjectileProxy)CreateProxy(projectile);
				scaleProxy.Add(-projectileProxy.Damage);
			}
		}

		private void TakeStepInner()
		{
			var removed = new List<Subject>();
			foreach (var collision in Playground.GetCollision())
			{
				CollisionHandle(collision.Item1, collision.Item2);
				CollisionHandle(collision.Item2, collision.Item1);

				((CollidedProxy?)CreateProxy(collision.Item1.Components[Playground.COLLIDED_NAME]))?.Break();
				((CollidedProxy?)CreateProxy(collision.Item2.Components[Playground.COLLIDED_NAME]))?.Break();

				if (collision.Item1.Components[Playground.PROJECTILE_NAME] is { })
				{
					removed.Add(collision.Item1);
				}

				if (collision.Item2.Components[Playground.PROJECTILE_NAME] is { })
				{
					removed.Add(collision.Item2);
				}
			}

			var playgroundProxy = (PlaygroundProxy)CreateProxy(Playground);
			foreach (var subject in removed)
			{
				playgroundProxy.Subjects.Remove(subject);
			}

			foreach (var subject in Playground.Subjects
				.Select(x => (SubjectProxy<Subject>)CreateProxy(x))
				.ToArray()
			)
			{
				if (subject.GetComponentProxy<CollidedProxy>(Playground.COLLIDED_NAME) is { } collided)
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
			RegisterProxyType<ProjectileProxy, Projectile>(x => new ProjectileProxy(this, x));
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
			var script = new Script();

			script.Globals["playground"] = Playground;

			script.Globals[TAKE_STEP_NAME] = (Action)TakeStepInner;
			script.Globals.Set(ENUMERATE_NAME, DynValue.NewCallback(Enumerate));
			script.DoString(scriptText);
		}
	}
}
