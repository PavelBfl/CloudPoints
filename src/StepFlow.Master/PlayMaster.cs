using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using MoonSharp.Interpreter;
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

		public Playground Playground { get; } = new Playground();

		public IComponent CreateComponent(string componentName)
		{
			return componentName switch
			{
				Playground.COLLIDED_NAME => new Collided(),
				"Scheduled" => new Scheduled(),
				"Strength" => new Scale(),
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep()
		{
			Execute(@"
			collision = playground.GetCollision();

			for _, collisionUnit in enumerate(collision) do

				strengthFirst = collisionUnit.Item1.GetComponent(""Strength"")
				if strengthFirst != null then
					strengthFirst.Add(collisionUnit.Item2.Damage)
				end

				strengthSecond = collisionUnit.Item2.GetComponent(""Strength"")
				if strengthSecond != null then
					strengthSecond.Add(collisionUnit.Item1.Damage)
				end
			end

			for _, subject in enumerate(playground.Subjects) do
				collided = subject.GetComponent(""Collided"")
				collided.Move()
			end
			");
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

			UserData.RegisterType<(Collided, Collided)>();
			RegisterList<(Collided, Collided)>();

			UserData.RegisterProxyType<PlaygroundProxy, Playground>(x => new PlaygroundProxy(this, x));

			UserData.RegisterProxyType<SubjectProxy<Subject>, Subject>(x => new SubjectProxy<Subject>(this, x));
			RegisterList<Subject>();

			UserData.RegisterProxyType<SubjectsCollectionProxy, IList<Subject>>(x => new SubjectsCollectionProxy(this, x));
			UserData.RegisterProxyType<BorderedProxy, Bordered>(x => new BorderedProxy(this, x));
			UserData.RegisterProxyType<CellProxy, Cell>(x => new CellProxy(this, x));

			UserData.RegisterProxyType<ScaleProxy, Scale>(x => new ScaleProxy(this, x));
			UserData.RegisterProxyType<CollidedProxy, Collided>(x => new CollidedProxy(this, x));
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
			script.Globals.Set("enumerate", DynValue.NewCallback(Enumerate));
			script.Globals["debug"] = (Action<object>)Debug;
			script.DoString(scriptText);
		}

		private void Debug(object obj)
		{
			
		}
	}
}
