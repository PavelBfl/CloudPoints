using System.Drawing;
using StepFlow.Common.Exceptions;
using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	internal sealed class ProjectileBuilderHandler : HandlerBase
	{
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

		public ProjectileBuilderHandler(PlayMaster owner) : base(owner)
		{
		}

		protected override void HandleInner(IComponentProxy component)
		{
			var ownerCollided = Subject.GetComponent(Master.Components.Names.COLLIDED);
			if (ownerCollided is ICollidedProxy { Current: { } current })
			{
				var projectileSettings = (IProjectileSettingsProxy)Subject.GetComponentRequired(Master.Components.Names.PROJECTILE_SETTINGS);

				var playground = Subject.Playground;

				var subject = playground.CreateSubject();
				playground.Subjects.Add(subject);
				var collided = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.COLLIDED);

				var bordered = playground.CreateBordered();

				var pivot = GetPivot(current.Target.Border, projectileSettings.Course);
				var projectileBorder = CreateRectangle(projectileSettings.Course.Invert(), pivot, new Size(projectileSettings.Size, projectileSettings.Size));
				projectileBorder.Offset(projectileSettings.Course.ToOffset());

				bordered.AddCell(projectileBorder);
				collided.Current = bordered;
				collided.Collision.Add(subject.AddComponent(Master.Components.Handlers.REMOVE_SUBJECT));

				var projectile = (IDamageProxy)subject.AddComponent(Master.Components.Types.DAMAGE, Master.Components.Names.DAMAGE);
				projectile.Value = projectileSettings.Damage;
				foreach (var kind in projectileSettings.Kind)
				{
					projectile.Kind.Add(kind);
				}

				var scheduler = (IScheduledProxy)subject.AddComponent(Master.Components.Types.SCHEDULER);
				for (var i = 0; i < 100; i++)
				{
					scheduler.SetCourse(projectileSettings.Course);
				}
			}
		}
	}
}
