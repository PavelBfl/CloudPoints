using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Collision;
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
				"Scheduled" => new Scheduled(),
				"Strength" => new Scale(),
				_ => throw new InvalidOperationException(),
			};
		}

		public void TakeStep()
		{
			try
			{
				// TODO Реализовать Enumerable т.к. lua может вызвать метод Reset который не поддерживают Linq
				Execute(@"
				collision = playground.GetCollision();

				for _, collisionUnit in enumerate(collision) do
					fullDamage = 0;

					for _, piece in enumerate(collisionUnit) do
						fullDamage = fullDamage + piece.CollisionDamage;
					end

					for _, piece in enumerate(collisionUnit) do
						strength = piece.GetComponent(""Strength"");
						strength.Add(-(fullDamage - piece.CollisionDamage));
					end
				end
			");
			}
			catch (Exception e)
			{

				throw;
			}

			//PushToAxis(Scheduler.Queue, Time);

			//foreach (var node in Place.Values)
			//{
			//	PushToAxis(node.Scheduler.Queue, Time);
			//}

			//foreach (var piece in Pieces)
			//{
			//	PushToAxis(piece.Scheduler.Queue, Time);
			//}

			//var collision = GetCollision();

			//foreach (var collisionUnit in collision)
			//{
			//	var fullDamage = collisionUnit.Sum(x => x.CollisionDamage);

			//	foreach (var piece in collisionUnit)
			//	{
			//		var addResult = piece.Strength.Add(-(fullDamage - piece.CollisionDamage));
			//		if (addResult == StrengthState.Min)
			//		{
			//			Pieces.Remove(piece);
			//		}
			//		else
			//		{
			//			Clear(piece);
			//		}
			//	}
			//}

			//foreach (var piece in Pieces)
			//{
			//	TakeStep(piece);
			//}

			//Time++;
		}

		private static void RegisterReadOnlyList<T>()
		{
			UserData.RegisterType<IReadOnlyList<T>>();
			UserData.RegisterType<IReadOnlyCollection<T>>();
			UserData.RegisterType<IEnumerable<T>>();
		}

		private void InitLua()
		{
			UserData.RegisterType<IEnumerator>();

			UserData.RegisterType<CollisionResult>();
			UserData.RegisterType<PairCollision>();
			UserData.RegisterType<CrashCollision>();

			RegisterReadOnlyList<CrashCollision>();
			RegisterReadOnlyList<PairCollision>();
			RegisterReadOnlyList<Piece>();
			RegisterReadOnlyList<IReadOnlyList<Piece>>();

			UserData.RegisterProxyType<NodeProxy, Node>(x => new NodeProxy(this, x));
			UserData.RegisterProxyType<PieceProxy, Piece>(x => new PieceProxy(this, x));
			UserData.RegisterProxyType<PiecesCollectionProxy, PiecesCollection>(x => new PiecesCollectionProxy(this, x));
			UserData.RegisterProxyType<PlaceProxy, Place>(x => new PlaceProxy(this, x));
			UserData.RegisterProxyType<PlaygroundProxy, Playground>(x => new PlaygroundProxy(this, x));

			UserData.RegisterProxyType<ScaleProxy, Scale>(x => new ScaleProxy(this, x));
		}

		private static string GetEnumerateIteration()
		{
			return $@"function {ENUMERATE_ITERATION_NAME}(enumerable, enumerator)
				if enumerator.MoveNext() then
					return enumerator, enumerator.Current
				end
			end";
		}

		private static string GetEnumerate()
		{
			return @$"function {ENUMERATE_NAME}(enumerable)
				return {ENUMERATE_ITERATION_NAME}, enumerable, enumerable.GetEnumerator()
			end";
		}

		public const string ENUMERATE_ITERATION_NAME = "enumerateIteration";
		public const string ENUMERATE_NAME = "enumerate";

		public void Execute(string scriptText)
		{
			var script = new Script();

			script.Globals["playground"] = Playground;
			script.Globals["Debug"] = (Action<object?>)Debug;
			script.DoString(
				GetEnumerateIteration() +
				Environment.NewLine +
				GetEnumerate() +
				Environment.NewLine +
				scriptText
			);
		}

		private static void Debug(object? obj)
		{
			
		}
	}
}
