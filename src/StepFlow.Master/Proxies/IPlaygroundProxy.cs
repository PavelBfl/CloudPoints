using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy
	{
		IList<ISubjectProxy> Subjects { get; }

		IBorderedProxy CreateBordered();
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
		ISubjectProxy CreateSubject();
		IEnumerable<(ISubjectProxy, ISubjectProxy)> GetCollision();
	}
}
