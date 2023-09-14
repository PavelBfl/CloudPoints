using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	public interface ICollisionHandler : IChild
	{
		void Collision(ISubjectProxy main, ISubjectProxy other);
	}
}
