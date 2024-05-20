using System.Numerics;
using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Actions
{
	public interface ISetCourseProxy : IActionBaseProxy<SetCourse>
	{
		Collided Collided { get; }

		Vector2 Course { get; }
	}

	internal sealed class SetCourseProxy : ActionBaseProxy<SetCourse>, ISetCourseProxy
	{
		public SetCourseProxy(PlayMaster owner, SetCourse target) : base(owner, target)
		{
		}

		public Collided Collided { get => Target.GetCollidedRequired(); }

		public Vector2 Course { get => Target.Course; }

		public override void Execute()
		{
			var collidedProxy = (ICollidedProxy)Owner.CreateProxy(Collided);
			collidedProxy.SetPosition(collidedProxy.Position + Course, true);
		}
	}
}
