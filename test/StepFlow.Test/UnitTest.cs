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
				bordered = playground.CreateBordered()
				bordered.AddCell(playground.CreateRectangle(0, 0, 5, 6))
				collided.Current = bordered
				collided.Offset(playground.CreatePoint(5, 5))
				collided.Damage = 5
				subject.AddComponent(""Strength"")
				strength = subject.GetComponent(""Strength"")
				strength.Max = 100
				strength.Value = 100
				playground.Subjects.Add(subject)
			");

			master.Execute(@"
				subject = playground.CreateSubject()
				subject.AddComponent(""Collided"")
				collided = subject.GetComponent(""Collided"")
				bordered = playground.CreateBordered()
				bordered.AddCell(playground.CreateRectangle(0, 0, 20, 10))
				collided.Current = bordered
				collided.Offset(playground.CreatePoint(0, 0))
				collided.Damage = 10
				subject.AddComponent(""Strength"")
				strength = subject.GetComponent(""Strength"")
				strength.Max = 1000
				strength.Value = 1000
				playground.Subjects.Add(subject)
			");

			master.TakeStep();
		}
	}
}