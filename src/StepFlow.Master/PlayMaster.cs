using System;
using System.Collections.Generic;
using System.ComponentModel;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		public PlayMaster()
		{
			Playground = new PlaygroundCmd(this, new Playground());

			InitLua();
		}

		public IAxis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public IPlaygroundCmd Playground { get; }

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

		private void InitLua()
		{
			UserData.RegisterType<IPlaygroundCmd>();
			UserData.RegisterType<IContainerCmd<Playground>>();

			UserData.RegisterType<ICollectionCmd<IPieceCmd>>();
			UserData.RegisterType<ICollection<IPieceCmd>>();

			UserData.RegisterType<IPlaceCmd>();
			UserData.RegisterType<INodeCmd>();
			UserData.RegisterType<IParticleCmd<Node>>();
			UserData.RegisterType<ISubjectCmd<Node>>();
			UserData.RegisterType<IContainerCmd<Node>>();

			UserData.RegisterType<IPieceCmd>();
			UserData.RegisterType<IParticleCmd<Piece>>();
			UserData.RegisterType<ISubjectCmd<Piece>>();
			UserData.RegisterType<IContainerCmd<Piece>>();
		}

		public void Execute(string scriptText)
		{
			var script = new Script();

			script.Globals["playground"] = Playground;
			script.DoString(scriptText);
		}
	}
}
