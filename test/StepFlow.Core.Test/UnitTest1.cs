using StepFlow.Core.Components;

namespace StepFlow.Core.Test;

public class UnitTest1
{
	[Fact]
	public void Test1()
	{
		var playground = new Playground();

		var subject0 = new Subject(playground);
		var border0 = new Bordered();
		border0.AddCell(new(0, 0, 3, 2));
		var collided0 = new Collided()
		{
			Current = border0,
			Offset = new(1, 0),
		};
		subject0.Add(collided0, Playground.COLLIDED_NAME);
		playground.Subjects.Add(subject0);

		var subject1 = new Subject(playground);
		var border1 = new Bordered();
		border1.AddCell(new(3, 0, 1, 1));
		var collided1 = new Collided()
		{
			Current = border1,
			Offset = new(0),
		};
		subject1.Add(collided1, Playground.COLLIDED_NAME);
		playground.Subjects.Add(subject1);

		var result = playground.GetCollision().ToArray();
	}
}