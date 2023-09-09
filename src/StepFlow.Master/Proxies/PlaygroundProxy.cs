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

		[MoonSharpHidden]
		public SubjectProxy CreateSubjectProxy(bool addContainer = true)
		{
			var result = (SubjectProxy)Owner.CreateProxy(CreateSubject());
			if (addContainer)
			{
				Subjects.Add(result.Target);
			}
			return result;
		}

		public ICollection<ISubjectProxy> Subjects => new CollectionProxy<Subject, ICollection<Subject>, ISubjectProxy>(Owner, Target.Subjects);

		public Subject CreateSubject() => new Subject(Target);

		public IEnumerable<(Subject, Subject)> GetCollision() => Target.GetCollision();

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

		public Bordered CreateBordered() => new Bordered();
	}
}
