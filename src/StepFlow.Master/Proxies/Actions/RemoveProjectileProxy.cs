using StepFlow.Core.Actions;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Actions
{
	public interface IRemoveItemProxy : IActionBaseProxy<RemoveItem>
	{
		Material? Item { get; set; }
	}

	internal sealed class RemoveProjectileProxy : ActionBaseProxy<RemoveItem>, IRemoveItemProxy
	{
		public RemoveProjectileProxy(PlayMaster owner, RemoveItem target) : base(owner, target)
		{
		}

		public Material? Item { get => Target.Item; set => SetValue(value); }

		public override void Execute()
		{
			if (Item is { } projectile)
			{
				var items = Owner.CreateCollectionUsedProxy(Owner.Playground.Items);
				items.Remove(projectile);
			}
		}
	}
}
