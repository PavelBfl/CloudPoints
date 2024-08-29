namespace StepFlow.Intersection.Test;

public class ShapeTest
{
	public static IEnumerable<object[]> GetShape()
	{
		var random = new Random(0);

		for (var i = 0; i < 10; i++)
		{
			yield return new object[] { Extensions.CreateShape(random) };
		}
	}

	[Theory]
	[MemberData(nameof(GetShape))]
	public void Enable_IsEnable_True(ShapeBuilder shapeBuilder)
	{
		var context = new Context();
		var shape = shapeBuilder.Create(context);
		shape.Enable();

		Assert.True(shape.IsEnable);
	}

	[Theory]
	[MemberData(nameof(GetShape))]
	public void Enable_ContextContains_True(ShapeBuilder shapeBuilder)
	{
		var context = new Context();
		var shape = shapeBuilder.Create(context);
		shape.Enable();

		Assert.Contains(shape, context);
	}

	[Theory]
	[MemberData(nameof(GetShape))]
	public void Disable_IsEnable_False(ShapeBuilder shapeBuilder)
	{
		var context = new Context();
		var shape = shapeBuilder.Create(context);
		shape.Enable();
		shape.Disable();

		Assert.False(shape.IsEnable);
	}

	[Theory]
	[MemberData(nameof(GetShape))]
	public void Disable_ContextContains_False(ShapeBuilder shapeBuilder)
	{
		var context = new Context();
		var shape = shapeBuilder.Create(context);
		shape.Enable();
		shape.Disable();

		Assert.DoesNotContain(shape, context);
	}

	[Theory]
	[MemberData(nameof(GetShape))]
	public void Contains_Default_True(ShapeBuilder shapeBuilder)
	{
		var context = new Context();
		var shape = shapeBuilder.Create(context);

		Assert.All(shape, x => Assert.True(shape.Bounds.Contains(x)));
	}
}
