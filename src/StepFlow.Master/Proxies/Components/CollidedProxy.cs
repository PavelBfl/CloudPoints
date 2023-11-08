using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Border;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<ICollided>
	{
		IBorderedProxy? Current { get; set; }

		IBorderedProxy? Next { get; set; }
	}

	internal sealed class CollidedProxy : ProxyBase<ICollided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, ICollided target) : base(owner, target)
		{
		}

		public IBorderedProxy? Current { get => (IBorderedProxy?)Owner.CreateProxy(Target.Current); set => SetValue(x => x.Current, value?.Target); }

		public IBorderedProxy? Next { get => (IBorderedProxy?)Owner.CreateProxy(Target.Next); set => SetValue(x => x.Next, value?.Target); }
	}
}
