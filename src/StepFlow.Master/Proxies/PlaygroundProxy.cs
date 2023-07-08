using System.Drawing;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Collision;

namespace StepFlow.Master.Proxies
{
	public sealed class PlaygroundProxy : ContainerProxy<Playground>
	{
		[MoonSharpHidden]
		public PlaygroundProxy(PlayMaster owner, Playground source) : base(owner, source)
		{
		}

		public PiecesCollection Pieces => Target.Pieces;

		public Place Place => Target.Place;

		public CollisionResult GetCollision() => Target.GetCollision();

		public Node CreateNode(int x, int y) => new Node(Target, new Point(x, y));

		public Piece CreatePiece() => new Piece(Target);
	}
}
