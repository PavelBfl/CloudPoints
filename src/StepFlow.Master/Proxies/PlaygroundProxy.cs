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
			foreach (var collision in Target.GetCollision())
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
			var collided = (ICollidedProxy)subject.AddComponent(Playground.COLLIDED_NAME);
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
			var collided = (ICollidedProxy)subject.AddComponent(Playground.COLLIDED_NAME);
			var bordered = Owner.CreateProxy(new Bordered());
			bordered.AddCell(rectangle);
			collided.Current = bordered;
			var strength = (IScaleProxy)subject.AddComponent(Playground.STRENGTH_NAME);
			strength.Max = strengthValue;
			strength.Value = strengthValue;
			subject.AddComponent(Playground.SCHEDULER_NAME);
			var damage = (ICollisionDamageProxy)subject.AddComponent(Playground.COLLISION_DAMAGE_NAME);
			damage.Damage = 1;
			subject.AddComponent(PlayMaster.COLLISION_HANDLE);
			Subjects.Add(subject);
		}
	}
}
