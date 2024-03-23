using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Intersection;

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

		public Collided? Collided { get => Target.Collided; set => SetValue(value); }

		public Course Course { get => Target.Course; set => SetValue(value); }

		Subject IProxyBase<Subject>.Target => throw new System.NotImplementedException();

		public void Execute()
		{
			if (Collided is { })
			{
				var offset = Course.ToOffset();
				var next = Collided.Current.AsEnumerable().Offset(offset);

				var collidedProxy = (ICollidedProxy)Owner.CreateProxy(Collided);
				collidedProxy.NextProxy.Reset(next);
				collidedProxy.IsMove = true;
			}
		}
	}
}
