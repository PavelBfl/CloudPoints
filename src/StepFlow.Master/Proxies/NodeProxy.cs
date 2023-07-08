using MoonSharp.Interpreter;
using StepFlow.Core;

namespace StepFlow.Master.Proxies
{
	public sealed class NodeProxy : SubjectProxy<Node>
	{
		[MoonSharpHidden]
		public NodeProxy(PlayMaster owner, Node source) : base(owner, source)
		{
		}

		public OccupiersCollection Occupiers => Target.Occupiers;
	}
}
