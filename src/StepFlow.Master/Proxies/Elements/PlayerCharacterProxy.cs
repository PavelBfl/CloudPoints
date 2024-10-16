using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Domains.Elements;
using StepFlow.Domains.States;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlayerCharacterProxy : IMaterialProxy<PlayerCharacter>
	{
		Scale Cooldown { get; set; }

		IList<ItemKind> Items { get; }
		int ActiveTarget { get; set; }

		void CreateProjectile(float radians);

		void CopyFrom(PlayerCharacterDto original)
		{
			NullValidate.ThrowIfArgumentNull(original, nameof(original));

			((IMaterialProxy<PlayerCharacter>)this).CopyFrom(original);
			Cooldown = original.Cooldown;
			var itemsProxy = Owner.CreateListProxy(Items);
			itemsProxy.Clear();
			itemsProxy.AddRange(original.Items);
		}

		bool CanJump();
	}

	internal sealed class PlayerCharacterProxy : MaterialProxy<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
		}

		public Scale Cooldown { get => Target.Cooldown; set => SetValue(value); }

		public int ActiveTarget { get => Target.ActiveTarget; set => SetValue(value); }

		public IList<ItemKind> Items => Target.Items;

		public override void OnTick()
		{
			base.OnTick();

			if (Strength.Value == 0)
			{
				Owner.GetPlaygroundItemsProxy().Remove(Target);
			}
			else
			{
				Cooldown--;
			}
		}

		protected override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (otherMaterial is Item item)
			{
				Owner.GetPlaygroundItemsProxy().Remove(item);
				var itemBody = (ICollidedProxy?)Owner.CreateProxy(item.Body);
				itemBody?.Clear();
				Owner.CreateListProxy(Target.Items).Add(item.Kind);

				Speed += item.Speed;
				Cooldown = Scale.CreateByMin(Cooldown.Max - item.AttackCooldown);
				Strength += item.AddStrength;
			}
			else if ((otherMaterial as Projectile)?.Immunity.Contains(Target) != true)
			{
				base.Collision(thisCollided, otherMaterial, otherCollided);
			}
		}

		public void CreateProjectile(float radians)
		{
			if (Cooldown.Value == 0)
			{
				var currentSkillKind = Items[ActiveTarget];
				var currentSkill = Target.Context.Items[currentSkillKind];
				var center = Target.Body.GetCurrentRequired().Bounds.GetCenter();
				foreach (var projectileSource in currentSkill.Projectiles)
				{
					var projectile = new Projectile(Target.Context, projectileSource)
					{
						Immunity = { Target },
					};

					projectile.Body.Current = Owner.CreateShape(projectile.Body.GetCurrentRequired().Offset(center));
					if (projectile.Route is { } route)
					{
						var matrixTransform = Matrix3x2.CreateRotation(radians) * Matrix3x2.CreateTranslation(center.X, center.Y);
						for (var i = 0; i < route.Path.Count; i++)
						{
							route.Path[i] = route.Path[i].Transform(matrixTransform);
						}
					}

					Owner.GetPlaygroundItemsProxy().Add(projectile);
				}

				var statesProxy = Owner.CreateCollectionProxy(States);
				foreach (var stateDto in currentSkill.StatesSettings)
				{
					var state = new State(Target.Context, stateDto);
					switch (state.Kind)
					{
						case StateKind.Dash:
							var dashCourse = new Vector2(state.Arg0, state.Arg1);
							dashCourse = Vector2.Transform(dashCourse, Matrix3x2.CreateRotation(radians));
							state.Arg0 = dashCourse.X;
							state.Arg1 = dashCourse.Y;
							break;
					}

					statesProxy.Add(state);
				}

				Cooldown = Scale.CreateByMax(currentSkill.AttackCooldown);
			}
		}

		public bool CanJump() => RigidExists(new Point(0, 1));
	}
}
