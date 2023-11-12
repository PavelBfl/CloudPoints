using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Elements;
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

		public Playground Playground { get; } = new Playground(new Context());

		public IPlaygroundProxy GetPlaygroundProxy() => new PlaygroundProxy(this, Playground);

		[return: NotNullIfNotNull("value")]
		public object? CreateProxy(object? value)
		{
			return value switch
			{
				Playground instance => new PlaygroundProxy(this, instance),
				PlayerCharacter instance => new PlayerCharacterProxy(this, instance),
				Obstruction instance => new ObstructionProxy(this, instance),
				Cell instance => new CellProxy(this, instance),
				SetCourse instance => new SetCourseProxy(this, instance),
				Turn instance => new TurnProxy(this, instance),
				null => null,
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep() => Execute(TAKE_STEP_CALL);

		private void TakeStepInner()
		{
			var collideds = Playground.GetAllContent().OfType<ICollided>();

			foreach (var collision in Playground.GetCollision(collideds).ToArray())
			{
				var collided1Proxy = (ICollidedProxy)CreateProxy(collision.Item1);
				var collided2Proxy = (ICollidedProxy)CreateProxy(collision.Item2);

				collided1Proxy.Collision(collided2Proxy);
				collided2Proxy.Collision(collided1Proxy);
			}

			foreach (var collision in Playground.GetAllContent()
				.OfType<ICollided>()
				.Select(x => (ICollidedProxy)CreateProxy(x))
				.ToArray()
			)
			{
				collision.Move();
			}

			Time++;

			foreach (var collision in Playground.GetAllContent()
				.OfType<IScheduled>()
				.Select(x => (IScheduledProxy)CreateProxy(x))
				.ToArray()
			)
			{
				collision.Dequeue();
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

			RegisterList<IPlaygroundProxy>();
			RegisterList<ICollidedProxy>();
			RegisterList<IScaleProxy>();
			RegisterList<IScheduledProxy>();

			RegisterList<IPlayerCharacterProxy>();
			RegisterList<IObstructionProxy>();
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
