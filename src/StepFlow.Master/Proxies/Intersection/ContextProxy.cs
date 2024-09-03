using System.Collections.Generic;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IContextProxy : IProxyBase<Context>, ICollection<Shape>
	{
		
	}

	internal sealed class ContextProxy : CollectionProxy<Shape, Context>, IContextProxy
	{
		public ContextProxy(PlayMaster owner, Context target) : base(owner, target)
		{
		}

		public IReadOnlyList<Relation> GetCollisions() => Target.GetCollisions();
	}
}
