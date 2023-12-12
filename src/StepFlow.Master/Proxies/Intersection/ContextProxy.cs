using System.Collections.Generic;
using StepFlow.Intersection;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Intersection
{
	public interface IContextProxy : IProxyBase<Context>, ICollection<IShapeBaseProxy<ShapeBase>>
	{
		
	}

	internal sealed class ContextProxy : CollectionItemsProxy<ShapeBase, Context, IShapeBaseProxy<ShapeBase>>, IContextProxy
	{
		public ContextProxy(PlayMaster owner, Context target) : base(owner, target)
		{
		}

		public IReadOnlyList<Relation> GetCollisions() => Target.GetCollisions();
	}
}
