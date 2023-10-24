using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master
{
	public class Handlers
	{
		public static void Collision(IHandlerProxy main, IComponentProxy component)
		{
			if (main.Subject.GetComponent(Master.Components.Names.STRENGTH) is IScaleProxy scale &&
				component.Subject.GetComponent(Master.Components.Names.DAMAGE) is IDamageProxy damage)
			{
				if (!damage.Kind.Any())
				{
					scale.Add(-damage.Value);
				}
				else
				{
					if (damage.Kind.Contains(PlayMaster.FIRE_DAMAGE))
					{
						scale.Add(-damage.Value * 2);
					}

					if (damage.Kind.Contains(PlayMaster.POISON_DAMAGET))
					{
						var poisonSubject = main.Subject.Playground.CreateSubject();
						var damageSubject = (IDamageProxy)poisonSubject.AddComponent(Master.Components.Types.DAMAGE, Master.Components.Names.DAMAGE);
						damageSubject.Value = damage.Value / 2;
						damageSubject.Kind.Add(PlayMaster.POISON_DAMAGET);
						var setDamageHandler = poisonSubject.AddHandler(nameof(Handlers.SetDamage));
						var removeSubjectHandler = poisonSubject.AddHandler(nameof(Handlers.RemoveSubject), true);

						var poisonScheduler = (IScheduledProxy)main.Subject.AddComponent(Master.Components.Types.SCHEDULER);
						for (var i = 0; i < 5; i++)
						{
							poisonScheduler.Add(60, setDamageHandler);
						}
						poisonScheduler.Add(0, removeSubjectHandler);
					}
				}
			}

			if (main.Subject.GetComponent(Master.Components.Names.COLLIDED) is ICollidedProxy mainCollided &&
				component is ICollidedProxy otherCollided)
			{
				if (otherCollided.IsRigid)
				{
					mainCollided.Break();
				}
			}

			if (main.Subject.GetComponent(Master.Components.Names.PROJECTILE_SETTINGS) is IProjectileSettingsProxy mainSettings &&
				component.Subject.GetComponent(Master.Components.Names.PROJECTILE_SETTINGS_SET) is IProjectileSettingsProxy otherSettings)
			{
				foreach (var kind in otherSettings.Kind)
				{
					mainSettings.Kind.Add(kind);
				}
			}
		}

		public static void ProjectileBuilder(IHandlerProxy main, IComponentProxy component)
		{
			var ownerCollided = main.Subject.GetComponent(Components.Names.COLLIDED);
			var projectileSettings = (IProjectileSettingsProxy)main.Subject.GetComponentRequired(Components.Names.PROJECTILE_SETTINGS);

			CreateProjectile(
				main.Subject,
				projectileSettings.Course,
				projectileSettings.Size,
				projectileSettings.Damage,
				projectileSettings.Kind,
				Enumerable.Range(0, 100).Select(_ => projectileSettings.Course)
			);
		}

		public static void RemoveComponent(IHandlerProxy main, IComponentProxy component)
		{
			component.Subject.RemoveComponent(component);
		}

		public static void RemoveSubject(IHandlerProxy main, IComponentProxy component)
		{
			main.Subject.Playground.Subjects.Remove(main.Subject);
		}

		public static void ScaleEmptyHandler(IHandlerProxy main, IComponentProxy component)
		{
			if (((IScaleProxy)component).Value <= 0)
			{
				main.Subject.Playground.Subjects.Remove(main.Subject);
			}
		}

		public static void SetCourseHandler(IHandlerProxy main, IComponentProxy component)
		{
			if (component.Subject.GetComponent(Master.Components.Names.COLLIDED) is ICollidedProxy collided)
			{
				var setCourse = main.Subject.GetComponents().OfType<ISetCourseProxy>().Single();
				var offset = setCourse.Course.ToOffset();
				collided.Offset(offset);
			}
		}

		public static void SetDamage(IHandlerProxy main, IComponentProxy component)
		{
			var damage = (IDamageProxy)main.Subject.GetComponentRequired(Master.Components.Names.DAMAGE);

			if (component.Subject.GetComponent(Master.Components.Names.STRENGTH) is IScaleProxy scale)
			{
				scale.Add(-damage.Value);
			}
		}

		public static void SentryGunReact(IHandlerProxy main, IComponentProxy component)
		{
			if (main.Subject.Target == component.Subject.Target)
			{
				return;
			}

			var sentryGun = main.Subject.GetComponents().OfType<ISentryGunProxy>().Single();
			if (sentryGun.Cooldown != 0)
			{
				return;
			}

			var otherState = (IStateProxy?)component.Subject.GetComponent(Components.Names.STATE);
			var state = (IStateProxy)main.Subject.GetComponentRequired(Components.Names.STATE);
			if (otherState?.Team == state.Team)
			{
				return;
			}

			sentryGun.CooldownReset();

			var beginCurrent = ((ICollidedProxy)main.Subject.GetComponent(Components.Names.COLLIDED)).Current;
			var endCurrent = ((ICollidedProxy)component.Subject.GetComponent(Components.Names.COLLIDED)).Current;

			var path = CourseExtensions.GetPath(GetCenter(beginCurrent.Target.Border), GetCenter(endCurrent.Target.Border)).ToArray();
			var projectile = CreateProjectile(
				main.Subject,
				path[0],
				10,
				1,
				Enumerable.Empty<string>(),
				path
			);

			var projectileState = (IStateProxy)projectile.AddComponent(Components.Types.STATE, Components.Names.STATE);
			projectileState.Team = state.Team;
		}

		public static void SentryGunOnFrame(IHandlerProxy main, IComponentProxy component)
		{
			var sentryGun = main.Subject.GetComponents().OfType<ISentryGunProxy>().Single();
			sentryGun.CooldownDecrement();
		}

		public static void RemoveSubjectIfRigid(IHandlerProxy main, IComponentProxy component)
		{
			if (((ICollidedProxy?)component)?.IsRigid ?? false)
			{
				RemoveSubject(main, component);
			}
		}

		private static Point GetCenter(Rectangle rectangle) => new Point(
			rectangle.X + rectangle.Width / 2,
			rectangle.Y + rectangle.Height / 2
		);

		private static Point GetPivot(Rectangle rectangle, Course position) => position switch
		{
			Course.Left => new Point(rectangle.Left, rectangle.Top + rectangle.Height / 2),
			Course.LeftTop => new Point(rectangle.Left, rectangle.Top),
			Course.Top => new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top),
			Course.RightTop => new Point(rectangle.Right, rectangle.Top),
			Course.Right => new Point(rectangle.Right, rectangle.Top + rectangle.Height / 2),
			Course.RightBottom => new Point(rectangle.Right, rectangle.Bottom),
			Course.Bottom => new Point(rectangle.Left + rectangle.Width / 2, rectangle.Bottom),
			Course.LeftBottom => new Point(rectangle.Left, rectangle.Bottom),
			_ => throw EnumNotSupportedException.Create(position),
		};

		private static Rectangle CreateRectangle(Course pivot, Point position, Size size) => pivot switch
		{
			Course.Left => new Rectangle(new Point(position.X, position.Y - size.Width / 2), size),
			Course.LeftTop => new Rectangle(position, size),
			Course.Top => new Rectangle(new Point(position.X - size.Width / 2, position.Y), size),
			Course.RightTop => new Rectangle(new Point(position.X - size.Width, position.Y), size),
			Course.Right => new Rectangle(new Point(position.X - size.Width, position.Y - size.Height / 2), size),
			Course.RightBottom => new Rectangle(new Point(position.X - size.Width, position.Y - size.Height), size),
			Course.Bottom => new Rectangle(new Point(position.X - size.Width / 2, position.Y - size.Height), size),
			Course.LeftBottom => new Rectangle(new Point(position.X, position.Y - size.Height), size),
			_ => throw EnumNotSupportedException.Create(pivot),
		};

		private static ISubjectProxy CreateProjectile(
			ISubjectProxy owner,
			Course begin,
			int size,
			float damage,
			IEnumerable<string> kinds,
			IEnumerable<Course> path
		)
		{
			var result = owner.Playground.CreateSubject();
			owner.Playground.Subjects.Add(result);
			var collided = (ICollidedProxy)result.AddComponent(Components.Types.COLLIDED, Components.Names.COLLIDED);

			var bordered = owner.Playground.CreateBordered();

			var current = ((ICollidedProxy)owner.GetComponent(Components.Names.COLLIDED)).Current;
			var pivot = GetPivot(current.Target.Border, begin);
			var projectileBorder = CreateRectangle(begin.Invert(), pivot, new Size(size, size));
			projectileBorder.Offset(begin.ToOffset());

			bordered.AddCell(projectileBorder);
			collided.Current = bordered;
			collided.Collision.Add(result.AddHandler(nameof(Handlers.RemoveSubjectIfRigid)));

			var projectile = (IDamageProxy)result.AddComponent(Components.Types.DAMAGE, Components.Names.DAMAGE);
			projectile.Value = damage;
			foreach (var kind in kinds)
			{
				projectile.Kind.Add(kind);
			}

			var scheduler = (IScheduledProxy)result.AddComponent(Components.Types.SCHEDULER);
			foreach (var step in path)
			{
				scheduler.SetCourse(step);
			}
			var removeSubject = result.AddHandler(nameof(Handlers.RemoveSubject), true);
			scheduler.Add(0, removeSubject);

			return result;
		}
	}
}
