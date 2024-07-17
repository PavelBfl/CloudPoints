using System.Linq;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Master.Proxies.States;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		Damage Damage { get; set; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public Damage Damage { get => Target.Damage; set => SetValue(value); }

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (!Target.Immunity.Contains(otherMaterial) && otherCollided.Collided.IsRigid)
			{
				var otherMaterialProxy = (IMaterialProxy<Material>)Owner.CreateProxy(otherMaterial);
				otherMaterialProxy.Strength -= Damage.Value;

				if (Damage.Push != Vector2.Zero && otherMaterial.Weight < Material.MAX_WEIGHT)
				{
					var totalCooldown = TimeTick.FromSeconds(1);
					if (otherMaterial.States.SingleOrDefault(x => x.Kind == StateKind.Push) is { } pushState)
					{
						var pushStateProxy = (IStateProxy)Owner.CreateProxy(pushState);
						pushStateProxy.TotalCooldown = totalCooldown;
						pushStateProxy.Vector = Damage.Push;
					}
					else
					{
						pushState = new State(Owner.Playground.Context)
						{
							Kind = StateKind.Push,
							TotalCooldown = totalCooldown,
							Arg0 = Damage.Push.X,
							Arg1 = Damage.Push.Y,
						};
						var statesProxy = Owner.CreateCollectionProxy(otherMaterial.States);
						statesProxy.Add(pushState);
					}
				}

				switch (Target.Reusable)
				{
					case ReusableKind.None:
						Owner.GetPlaygroundItemsProxy().Remove(Target);
						break;
					case ReusableKind.Save:
						Owner.CreateCollectionProxy(Target.Immunity).Add(otherMaterial);
						break;
				}
			}
		}
	}
}
