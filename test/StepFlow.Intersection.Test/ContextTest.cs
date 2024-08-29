namespace StepFlow.Intersection.Test;

public class ContextTest
{
	public static IEnumerable<object[]> GetShapes()
	{
		const int MAX_SHAPES = 20;
		var random = new Random(0);

		for (var i = 0; i < 10; i++)
		{
			var shapes = new ShapeBuilder[random.Next(1, MAX_SHAPES + 1)];
			for (var iShape = 0; iShape < shapes.Length; iShape++)
			{
				shapes[iShape] = Extensions.CreateShape(random);
			}

			yield return new object[] { shapes };
		}
	}

	[Theory]
	[MemberData(nameof(GetShapes))]
	public void Add_NewShapes_Count(ShapeBuilder[] shapes)
	{
		var context = new Context();
		foreach (var shape in shapes)
		{
			var shapeBase = shape.Create(context);
			shapeBase.Enable();
		}

		Assert.Equal(shapes.Length, context.Count);
	}

	[Theory]
	[MemberData(nameof(GetShapes))]
	public void Remove_NewShapes_Count(ShapeBuilder[] shapes)
	{
		var context = new Context();
		foreach (var shape in shapes)
		{
			var shapeBase = shape.Create(context);
			shapeBase.Enable();
		}

		context.First().Disable();

		Assert.Equal(shapes.Length - 1, context.Count);
	}

	[Theory]
	[MemberData(nameof(GetShapes))]
	public void IsEnable_NewShapes_AllTrue(ShapeBuilder[] shapes)
	{
		var context = new Context();
		foreach (var shape in shapes)
		{
			var shapeBase = shape.Create(context);
			shapeBase.Enable();
		}

		Assert.All(context, x => Assert.True(x.IsEnable));
	}

	[Theory]
	[MemberData(nameof(GetShapes))]
	public void IsEnable_NewShapes_AllFalse(ShapeBuilder[] shapes)
	{
		var context = new Context();
		foreach (var shape in shapes)
		{
			var shapeBase = shape.Create(context);
		}

		Assert.All(context, x => Assert.False(x.IsEnable));
	}
}