using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core.Border;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Border
{
	public interface IBorderedBaseProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : class, IBordered
	{
		Rectangle Border => Target.Border;

		void Offset(Point value);

		IEnumerable<IBorderedBaseProxy<IBordered>>? Childs => null;
	}

	internal abstract class BorderedBaseProxy<TTarget> : ProxyBase<TTarget>, IBorderedBaseProxy<TTarget>
		where TTarget : class, IBordered
	{
		public BorderedBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public abstract void Offset(Point value);
	}
}
