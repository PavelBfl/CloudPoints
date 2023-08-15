using System.Drawing;
using MoonSharp.Interpreter;
using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public sealed class PlaygroundProxy : ContainerProxy<Playground>
	{
		[MoonSharpHidden]
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public SubjectsCollectionProxy Subjects => new SubjectsCollectionProxy(Owner, Target.Subjects);

		public Subject CreateSubject() => new Subject(Target);

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);
	}
}
