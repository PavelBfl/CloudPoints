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

		public ISubjectProxy CreateSubject() => Owner.CreateProxy(new Subject(Target));

		public IEnumerable<(ISubjectProxy, ISubjectProxy)> GetCollision()
		{
			foreach (var collision in Target.GetCollision(Master.Components.Names.COLLIDED))
			{
				yield return (Owner.CreateProxy(collision.Item1), Owner.CreateProxy(collision.Item2));
			}
		}

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

		public IBorderedProxy CreateBordered() => Owner.CreateProxy(new Bordered());

		private void CreateWall(Rectangle rectangle)
		{
			var subject = CreateSubject();
			var collided = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.COLLIDED);
			var bordered = Owner.CreateProxy(new Bordered());
			bordered.AddCell(rectangle);
			collided.Current = bordered;
			Subjects.Add(subject);
		}

		public void CreateRoom(Rectangle rectangle, int width)
		{
			CreateWall(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, width));
			CreateWall(new Rectangle(rectangle.X, rectangle.Y, width, rectangle.Height));
			CreateWall(new Rectangle(rectangle.Right - width, rectangle.Y, width, rectangle.Height));
			CreateWall(new Rectangle(rectangle.X, rectangle.Bottom - width, rectangle.Width, width));
		}

		public void CreateItem(Rectangle rectangle, int strengthValue)
		{
			var subject = CreateSubject();
			var collided = (ICollidedProxy)subject.AddComponent(Master.Components.Types.COLLIDED, Master.Components.Names.COLLIDED);
			var bordered = Owner.CreateProxy(new Bordered());
			bordered.AddCell(rectangle);
			collided.Current = bordered;
			var strength = (IScaleProxy)subject.AddComponent(Master.Components.Types.SCALE, Master.Components.Names.STRENGTH);
			strength.Max = strengthValue;
			strength.Value = strengthValue;
			strength.ValueChange.Add(subject.AddComponent(Master.Components.Handlers.SCALE));
			subject.AddComponent(Master.Components.Types.SCHEDULER, Master.Components.Names.MAIN_SCHEDULER);
			var damage = (ICollisionDamageProxy)subject.AddComponent(Master.Components.Types.COLLISION_DAMAGE, Master.Components.Names.DAMAGE);
			damage.Damage = 1;
			collided.Collision.Add(subject.AddComponent(Master.Components.Handlers.COLLISION));
			Subjects.Add(subject);
		}
	}
}
