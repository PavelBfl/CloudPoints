namespace StepFlow.Core.Test;

public class UnitTest1
{
	[Fact]
	public void Test1()
	{
		var playground = new Playground();

		var cells0 = new CellsCollection(playground)
		{
			new(0, 0),
			new(1, 0),
			new(2, 0),
		};

		var cells1 = new CellsCollection(playground)
		{
			{ new(0, 1), new(0, 0) }
		};

		foreach (var cell in new[] { cells0, cells1 }.SelectMany(x => x))
		{
			playground.Cells.Add(cell);
		}

		var collisions = playground.DeclareCollision().ToArray();
	}
}