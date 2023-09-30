using StepFlow.Common.Exceptions;
using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	public sealed class CourseHandler : HandlerBase
	{
		public const long FLAT_FACTOR = 5;
		public const long DIAGONAL_FACTOR = 7;

		public static long GetFactor(Course course) => course switch
		{
			Course.Right => FLAT_FACTOR,
			Course.Left => FLAT_FACTOR,
			Course.Top => FLAT_FACTOR,
			Course.Bottom => FLAT_FACTOR,
			Course.RightTop => DIAGONAL_FACTOR,
			Course.RightBottom => DIAGONAL_FACTOR,
			Course.LeftTop => DIAGONAL_FACTOR,
			Course.LeftBottom => DIAGONAL_FACTOR,
			_ => throw EnumNotSupportedException.Create(course),
		};

		public CourseHandler(PlayMaster owner) : base(owner)
		{
		}

		public Course Course { get; set; }

		protected override void HandleInner(IComponentProxy component)
		{
			if (component.Subject.GetComponent(Master.Components.Names.COLLIDED) is ICollidedProxy collided)
			{
				var offset = Course.ToOffset();
				collided.Offset(offset);
			}
		}
	}
}
