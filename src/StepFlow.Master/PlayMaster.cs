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
				null => null,
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep() => Execute(TAKE_STEP_CALL);

		private void TakeStepInner()
		{
			foreach (var collision in Playground.GetMaterials()
				.Select(x => x.Scheduler)
				.OfType<Scheduled>()
				.Select(x => (IScheduledProxy)CreateProxy(x))
				.ToArray()
			)
			{
				collision.Dequeue();
			}

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

			RegisterList<ICellProxy>();
			RegisterList<IBorderedProxy>();

			RegisterList<IDamageProxy>();
			RegisterList<ICollidedProxy>();
			RegisterList<IScaleProxy>();
			RegisterList<IScheduledProxy>();
			RegisterList<ITurnProxy>();
			RegisterList<ISetCourseProxy>();

			RegisterList<IPlaygroundProxy>();
			RegisterList<IPlayerCharacterProxy>();
			RegisterList<IMaterialProxy<PlayerCharacter>>();
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
