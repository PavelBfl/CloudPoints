namespace StepFlow.Core.Test;

public class UnitTest1
{
	[Fact]
	public void Test1()
	{
		var bordered0 = new Bordered();
		bordered0.AddCell(new(0, 0, 1, 1));
		bordered0.AddCell(new(2, 0, 1, 1));

		var bordered1 = new Bordered();
		bordered1.AddCell(new(0, 0, 1, 1));

		var result = Playground.GetCollisions(new[] { bordered0, bordered1 }).ToArray();
	}
}