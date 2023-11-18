using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Border;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<ICollided>
	{
		IBorderedBaseProxy<IBordered>? Current { get; set; }

		IBorderedBaseProxy<IBordered>? Next { get; set; }

		bool IsMove { get; set; }

		void Collision(ICollidedProxy other);

		void Move()
		{
			if (IsMove)
			{
				Current = Next;
				Break();
			}
		}

		void Break()
		{
			Next = null;
			IsMove = false;
		}
	}
}
