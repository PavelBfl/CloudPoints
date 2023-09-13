using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	public interface ICollisionHandler : IIdentity
	{
		void Collision(ISubjectProxy main, ISubjectProxy other);
	}
}
