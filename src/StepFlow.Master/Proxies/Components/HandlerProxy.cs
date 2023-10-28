namespace StepFlow.Master.Proxies.Components
{
	[ComponentProxy(typeof(Core.Components.Handler), typeof(HandlerProxy), "Handler")]
	public interface IHandlerProxy : IComponentProxy
	{
		bool Disposable { get; set; }

		string? Reference { get; set; }

		void Handle(IComponentProxy component);
	}

	internal sealed class HandlerProxy : ComponentProxy<Core.Components.Handler>, IHandlerProxy
	{
		public HandlerProxy(PlayMaster owner, Core.Components.Handler target) : base(owner, target)
		{
		}

		public bool Disposable { get => Target.Disposable; set => SetValue(x => x.Disposable, value); }

		public string? Reference { get => Target.Reference; set => SetValue(x => x.Reference, value); }

		public void Handle(IComponentProxy component)
		{
			if (Reference is { })
			{
				var handler = Owner.Handlers[Reference];
				handler(this, component);
			}

			if (Disposable)
			{
				Subject.RemoveComponent(this);
			}
		}
	}
}
