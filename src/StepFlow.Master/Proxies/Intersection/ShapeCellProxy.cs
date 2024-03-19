using System.Drawing;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IShapeCellProxy : IShapeBaseProxy<ShapeCell>
	{
		Rectangle Border { get; set; }
	}

	internal sealed class ShapeCellProxy : ShapeBaseProxy<ShapeCell>, IShapeCellProxy
	{
		public ShapeCellProxy(PlayMaster owner, ShapeCell target) : base(owner, target)
		{
		}

		public Rectangle Border { get => Target.Border; set => SetValue(value); }
	}
}
