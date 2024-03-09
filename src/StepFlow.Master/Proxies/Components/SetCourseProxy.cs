using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface ISetCourseProxy : IProxyBase<SetCourse>, ITurnExecutor
	{
		Collided? Collided { get; set; }

		Course Course { get; set; }
	}

	internal sealed class SetCourseProxy : ProxyBase<SetCourse>, ISetCourseProxy
	{
		public SetCourseProxy(PlayMaster owner, SetCourse target) : base(owner, target)
		{
		}

		public Collided? Collided { get => Target.Collided; set => SetValue(x => x.Collided, value); }

		public Course Course { get => Target.Course; set => SetValue(x => x.Course, value); }

		Subject IProxyBase<Subject>.Target => throw new System.NotImplementedException();

		public void Execute()
		{
			if (Collided is { Current: { } current })
			{
				var offset = Course.ToOffset();
				var next = current.Clone(offset);

				var collidedProxy = (ICollidedProxy)Owner.CreateProxy(Collided);
				collidedProxy.Next = next;
				collidedProxy.IsMove = true;
			}
		}
	}
}
