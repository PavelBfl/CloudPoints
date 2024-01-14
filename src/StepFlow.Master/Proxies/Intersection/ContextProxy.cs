using System.Collections.Generic;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IContextProxy : IProxyBase<Context>, ICollection<ShapeBase>
	{
		
	}

	internal sealed class ContextProxy : CollectionProxy<ShapeBase, Context>, IContextProxy
	{
		public ContextProxy(PlayMaster owner, Context target) : base(owner, target)
		{
		}

		public IReadOnlyList<Relation> GetCollisions() => Target.GetCollisions();
	}
}
