using System.Drawing;
using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IComponentProxy
	{
		IBorderedProxy? Current { get; set; }

		IBorderedProxy? Next { get; set; }

		bool IsMoving { get; set; }
		string? CollidedEvent { get; set; }

		void Break();
		void CollidedHandle(ISubjectProxy other);
		void Move();

		bool Offset(Point value);
	}
}
