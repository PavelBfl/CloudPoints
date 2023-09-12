using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.Components.Custom;

namespace StepFlow.Master.Proxies
{
	public sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		public const string COLLISION_HANDLE = "Collision";
		public const string REMOVE_HANDLE = "Remove";

		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public IReadOnlyDictionary<string, IHandler> Handlers { get; } = new Dictionary<string, IHandler>()
		{
			{ COLLISION_HANDLE, new CollisionHandler() },
			{ REMOVE_HANDLE, new RemoveHandler() },
		};

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
	}
}
