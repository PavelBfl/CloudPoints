using System.Linq;
using System.Numerics;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Core.States;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Schedulers;
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
						var pushStateProxy = (IStateProxy<State>)Owner.CreateProxy(pushState);
						pushStateProxy.TotalCooldown = totalCooldown;
						pushStateProxy.Vector = Damage.Push;
					}
					else
					{
						pushState = new State()
						{
							Kind = StateKind.Push,
							TotalCooldown = totalCooldown,
							Arg0 = Damage.Push.X,
							Arg1 = Damage.Push.Y,
						};
						var statesProxy = Owner.CreateCollectionProxy(otherMaterial.States);
						statesProxy.Add(pushState);
					}

					//var controlVector = GetOrCreateControlVector(otherMaterial, Material.SHEDULER_INERTIA_NAME);
					//var courseVectorProxy = (ICourseVectorProxy)Owner.CreateProxy(controlVector.CourseVector);
					//courseVectorProxy.Value = Damage.Push;
					//var factor = 1f - (otherMaterial.Weight / (float)Material.MAX_WEIGHT);
					//courseVectorProxy.Delta = Matrix3x2.CreateScale(factor);
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

		private Material.CourseVectorPath GetOrCreateControlVector(Material material, string vectorName)
		{
			var result = material.GetCourseVector(vectorName);
			if (result is null)
			{
				var schedulersProxy = Owner.CreateCollectionProxy(material.Schedulers);
				var courseVector = new CourseVector()
				{
					Name = vectorName,
				};

				var schedulerVector = new SchedulerVector()
				{
					Collided = material.Body,
					Vectors = { courseVector },
				};

				var schedulerRunner = new SchedulerRunner()
				{
					Scheduler = schedulerVector,
				};

				result = new Material.CourseVectorPath(schedulerRunner, schedulerVector, courseVector);
				schedulersProxy.Add(schedulerRunner);
			}

			return result;
		}
	}
}
