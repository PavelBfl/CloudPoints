using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Intersection;

namespace StepFlow.Master.Proxies.Components
{
	public interface ISetCourseProxy : IProxyBase<SetCourse>, ITurnExecutor
	{
		ICollidedProxy? Collided { get; set; }

		Course Course { get; set; }
	}

	internal sealed class SetCourseProxy : ProxyBase<SetCourse>, ISetCourseProxy
	{
		public SetCourseProxy(PlayMaster owner, SetCourse target) : base(owner, target)
		{
		}

		public ICollidedProxy? Collided { get => (ICollidedProxy?)Owner.CreateProxy(Target.Collided); set => SetValue(x => x.Collided, value?.Target); }

		public Course Course { get => Target.Course; set => SetValue(x => x.Course, value); }

		public void Execute()
		{
			if (Collided is { Current: { } current })
			{
				var offset = Course.ToOffset();
				var next = current.Clone(offset);

				Collided.Next = next;
				Collided.IsMove = true;
			}
		}

		Subject IReadOnlyProxyBase<Subject>.Target => Target;
	}
}
