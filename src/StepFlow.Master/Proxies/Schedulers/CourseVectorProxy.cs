using System.Numerics;
using StepFlow.Core.Schedulers;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ICourseVectorProxy : IProxyBase<CourseVector>
	{
		Vector2 Value { get; set; }

		Matrix3x2 Delta { get; set; }
	}

	internal class CourseVectorProxy : ProxyBase<CourseVector>, ICourseVectorProxy
	{
		public CourseVectorProxy(PlayMaster owner, CourseVector target) : base(owner, target)
		{
		}

		public Vector2 Value { get => Target.Value; set => SetValue(value); }

		public Matrix3x2 Delta { get => Target.Delta; set => SetValue(value); }
	}
}
