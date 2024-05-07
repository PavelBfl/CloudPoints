using System.Linq;
using System.Numerics;
using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Actions
{
	public interface ISetCourseProxy : IActionBaseProxy<SetCourse>
	{
		Collided? Collided { get; set; }

		Vector2 Course { get; set; }
	}

	internal sealed class SetCourseProxy : ActionBaseProxy<SetCourse>, ISetCourseProxy
	{
		public SetCourseProxy(PlayMaster owner, SetCourse target) : base(owner, target)
		{
		}

		public Collided? Collided { get => Target.Collided; set => SetValue(value); }

		public Vector2 Course { get => Target.Course; set => SetValue(value); }

		public override void Execute()
		{
			if (Collided is { })
			{
				var next = Collided.Current.AsEnumerable().Offset(Course);

				var collidedProxy = (ICollidedProxy)Owner.CreateProxy(Collided);
				collidedProxy.NextProxy.Reset(next);
				collidedProxy.IsMove = true;
			}
		}
	}
}
