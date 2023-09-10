using System.Collections.Generic;
using System.Drawing;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		[MoonSharpHidden]
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public ICollection<ISubjectProxy> Subjects => new CollectionProxy<Subject, ICollection<Subject>, ISubjectProxy>(Owner, Target.Subjects);

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

		public Bordered CreateBordered() => new Bordered();
	}
}
