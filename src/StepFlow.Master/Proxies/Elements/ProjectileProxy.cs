using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		IProxyBase<Subject>? Creator { get; set; }

		IDamageProxy? Damage { get; set; }

		int CurrentPathIndex { get; set; }

		IList<Course> Path { get; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public IProxyBase<Subject>? Creator
		{
			get => (IProxyBase<Subject>?)Owner.CreateProxy(Target.Creator);
			set => SetValue(x => x.Creator, value?.Target);
		}

		public IDamageProxy? Damage
		{
			get => (IDamageProxy?)Owner.CreateProxy(Target.Damage);
			set => SetValue(x => x.Damage, value?.Target);
		}

		public int CurrentPathIndex { get => Target.CurrentPathIndex; set => SetValue(x => x.CurrentPathIndex, value); }

		public IList<Course> Path => CreateListProxy(Target.Path);

		public override void OnTick()
		{
			base.OnTick();

			if (CurrentPathIndex >= Path.Count)
			{
				Owner.GetPlaygroundProxy().Projectiles.Remove(this);
				Body.Current = null;
				Body.Next = null;
			}
			else if (CurrentAction is null)
			{
				SetCourse(Path[CurrentPathIndex]);
				CurrentPathIndex++;
			}
			
		}

		public override void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (Creator?.Target != otherMaterial.Target && otherCollided.IsRigid)
			{
				if (otherMaterial.Strength is { } strength)
				{
					strength.Add(-Damage.Value);
				}

				Owner.GetPlaygroundProxy().Projectiles.Remove(this);
				Body.Current = null;
				Body.Next = null;
			}
		}
	}
}
