namespace StepFlow.Master.Proxies.Components
{
	internal sealed class HandlerProxy : ComponentProxy<Core.Components.Handler>, IHandlerProxy
	{
		public HandlerProxy(PlayMaster owner, Core.Components.Handler target) : base(owner, target)
		{
		}

		public bool Disposable { get => Target.Disposable; set => SetValue(x => x.Disposable, value); }

		public string? Reference { get => Target.Reference; set => SetValue(x => x.Reference, value); }

		public void Handle(IComponentProxy component)
		{
			Owner.CallHandler(this, component);

			if (Disposable)
			{
				Subject.RemoveComponent(this);
			}
		}
	}
}
