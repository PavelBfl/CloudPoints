using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IComponentProxy
	{
		IBorderedProxy? Current { get; set; }

		IBorderedProxy? Next { get; set; }

		bool IsMoving { get; set; }

		ICollection<IHandlerProxy> Collision { get; }
		bool IsRigid { get; set; }

		void Break();

		void Move();

		bool Offset(Point value);
	}
}
