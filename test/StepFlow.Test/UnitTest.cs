using StepFlow.Core.Components;
using StepFlow.Master;

namespace StepFlow.Test
{
	public class UnitTest
	{
		[Fact]
		public void TestLua()
		{
			var master = new PlayMaster();

			master.Execute(@"
				subject = playground.CreateSubject()
				subject.AddComponent(""Collided"")
				collided = subject.GetComponent(""Collided"")
				collided.Size = playground.CreateRectangle(0, 0, 20, 10)
				collided.Offset = playground.CreatePoint(15, 20)
				playground.Subjects.Add(subject)
			");

			master.Execute(@"
				for x = 0, 2, 1
				do
					for y = 0, 2, 1
					do
						playground.place.Add(playground.createNode(x, y))
					end
				end
			");

			master.Execute(@"
				piece = playground.createPiece()
				playground.pieces.Add(piece)
				piece.AddComponent(""Strength"")
				piece.current = playground.place[0, 0]
				piece.next = playground.place[1, 0]
				piece.IsScheduledStep = true
				piece.CollisionDamage = 10
			");

			master.Execute(@"
				piece = playground.createPiece()
				playground.pieces.Add(piece)
				piece.AddComponent(""Strength"")
				piece.current = playground.place[1, 0]
				piece.next = playground.place[0, 0]
				piece.IsScheduledStep = true
				piece.CollisionDamage = 10
			");

			master.TakeStep();
		}
	}
}