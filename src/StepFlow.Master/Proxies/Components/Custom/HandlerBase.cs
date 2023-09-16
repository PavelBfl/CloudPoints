namespace StepFlow.Master.Proxies.Components.Custom
{
	internal abstract class HandlerBase : ComponentMaster, IHandler
	{
		protected HandlerBase(PlayMaster owner) : base(owner)
		{
		}

		public bool Disposable { get; set; }

		public void Handle(IComponentProxy component)
		{
			HandleInner(component);

			if (Disposable)
			{
				Subject.RemoveComponent(this);
			}
		}

		protected abstract void HandleInner(IComponentProxy component);
	}
}
