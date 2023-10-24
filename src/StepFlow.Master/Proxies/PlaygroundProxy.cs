using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public IList<ISubjectProxy> Subjects => new ListItemsProxy<Subject, IList<Subject>, ISubjectProxy>(Owner, Target.Subjects);

		public ISubjectProxy CreateSubject() => (ISubjectProxy)Owner.CreateProxy(new Subject(Target));

		public IEnumerable<(ICollidedProxy, ICollidedProxy)> GetCollision()
		{
			foreach (var collision in Target.GetCollision())
			{
				yield return ((ICollidedProxy)Owner.CreateProxy(collision.Item1), (ICollidedProxy)Owner.CreateProxy(collision.Item2));
			}
		}

		private IBorderedProxy CreateBorder(Rectangle rectangle)
		{
			var bordered = (IBorderedProxy)Owner.CreateProxy(new Bordered());
			bordered.AddCell(rectangle);
			return bordered;
		}

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

		public IBorderedProxy CreateBordered() => (IBorderedProxy)Owner.CreateProxy(new Bordered());

		private void CreateWall(Rectangle rectangle)
		{
			var subject = CreateSubject();
			subject.Name = "Wall";
			var collided = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.COLLIDED);
			collided.IsRigid = true;
			collided.Current = CreateBorder(rectangle);
			Subjects.Add(subject);
		}

		public void CreateRoom(Rectangle rectangle, int width)
		{
			CreateWall(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, width));
			CreateWall(new Rectangle(rectangle.X, rectangle.Y, width, rectangle.Height));
			CreateWall(new Rectangle(rectangle.Right - width, rectangle.Y, width, rectangle.Height));
			CreateWall(new Rectangle(rectangle.X, rectangle.Bottom - width, rectangle.Width, width));
		}

		public void CreateCharacter(Rectangle rectangle, int strengthValue)
		{
			var subject = CreateSubject();
			subject.Name = "Character";
			var collided = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.COLLIDED);
			collided.IsRigid = true;
			collided.Current = CreateBorder(rectangle);
			var strength = (IScaleProxy)subject.AddComponent(Master.Components.Types.SCALE, Master.Components.Names.STRENGTH);
			strength.Max = strengthValue;
			strength.Value = strengthValue;
			var scaleHandler = subject.AddHandler(nameof(Handlers.ScaleEmptyHandler));
			strength.ValueChange.Add(scaleHandler);
			subject.AddComponent(Master.Components.Types.SCHEDULER, Master.Components.Names.MAIN_SCHEDULER);
			var damage = (IDamageProxy)subject.AddComponent(Master.Components.Types.DAMAGE, Master.Components.Names.DAMAGE);
			damage.Value = 1;
			var collisionHandler = subject.AddHandler(nameof(Handlers.Collision));
			collided.Collision.Add(collisionHandler);
			var projectileSettings = (IProjectileSettingsProxy)subject.AddComponent(Master.Components.Types.PROJECTILE_SETTINGS, Master.Components.Names.PROJECTILE_SETTINGS);
			projectileSettings.Damage = 10;
			projectileSettings.Size = 10;
			Subjects.Add(subject);
		}

		public void CreateSentryGun(Rectangle size, Rectangle vision, int strengthValue)
		{
			var subject = CreateSubject();
			subject.Name = "SentryGun";
			Subjects.Add(subject);
			var state = (IStateProxy)subject.AddComponent(Master.Components.Types.STATE, Master.Components.Names.STATE);
			state.Team = 1;
			var strength = (IScaleProxy)subject.AddComponent(Master.Components.Types.SCALE, Master.Components.Names.STRENGTH);
			strength.Value = strengthValue;
			strength.Max = strengthValue;
			strength.ValueChange.Add(subject.AddHandler(nameof(Handlers.ScaleEmptyHandler)));
			subject.AddComponent(Master.Components.Types.SENTRY_GUN);
			var collided = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.COLLIDED);
			collided.IsRigid = true;
			collided.Current = CreateBorder(size);
			collided.Collision.Add(subject.AddHandler(nameof(Handlers.Collision)));
			var visionComponent = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.VISION);
			visionComponent.Current = CreateBorder(vision);
			visionComponent.Collision.Add(subject.AddHandler(nameof(Handlers.SentryGunReact)));

			var system = (ISystemProxy)subject.AddComponent(Master.Components.Types.SYSTEM);
			system.OnFrame.Add(subject.AddHandler(nameof(Handlers.SentryGunOnFrame)));
		}

		public void CreateItem(Rectangle rectangle, string kind)
		{
			var subject = CreateSubject();
			subject.Name = "Item";
			var collided = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.COLLIDED);
			collided.Current = CreateBorder(rectangle);
			var removeSubjectHandler = subject.AddHandler(nameof(Handlers.RemoveSubject));
			collided.Collision.Add(removeSubjectHandler);
			var projectileSettings = (IProjectileSettingsProxy)subject.AddComponent(Master.Components.Types.PROJECTILE_SETTINGS, Master.Components.Names.PROJECTILE_SETTINGS_SET);
			projectileSettings.Kind.Add(kind);

			Subjects.Add(subject);
		}
	}
}
