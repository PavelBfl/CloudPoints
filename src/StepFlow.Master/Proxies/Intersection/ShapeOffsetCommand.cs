using System;
using System.Drawing;
using StepFlow.Intersection;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Intersection
{
	public sealed class ShapeOffsetCommand : ICommand
	{
		public ShapeOffsetCommand(Shape shape, Point value)
		{
			Shape = shape ?? throw new ArgumentNullException(nameof(shape));
			Value = value;
		}

		public Shape Shape { get; }

		public Point Value { get; }

		public void Execute() => Shape.Offset(Value);

		public void Revert() => Shape.Offset(new Point(-Value.X, -Value.Y));
	}
}
