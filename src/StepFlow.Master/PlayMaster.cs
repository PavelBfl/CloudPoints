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
			Execute(@"
			collision = playground.GetCollision();

			for _, collisionUnit in enumerate(collision) do
				fullDamage = 0;

				for _, piece in enumerate(collisionUnit) do
					fullDamage = fullDamage + piece.CollisionDamage;
				end

				removing = {}
				for _, piece in enumerate(collisionUnit) do
					strength = piece.GetComponent(""Strength"");
					state = strength.Add(-(fullDamage - piece.CollisionDamage));
					if state == 1 then
						table.insert(removing, piece)
					end
				end

				for _, piece in pairs(removing) do
					playground.Pieces.Remove(piece)
				end
			end
			");
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
			script.DoString(scriptText);
		}
	}
}
