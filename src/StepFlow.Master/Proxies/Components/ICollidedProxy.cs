using StepFlow.Core.Border;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Border;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollidedProxy : IProxyBase<Collided>
	{
		IBorderedBaseProxy<IBordered>? Current { get; set; }

		IBorderedBaseProxy<IBordered>? Next { get; set; }

		bool IsMove { get; set; }

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

	internal sealed class CollidedProxy : ProxyBase<Collided>, ICollidedProxy
	{
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public IBorderedBaseProxy<IBordered>? Current
		{
			get => (IBorderedBaseProxy<IBordered>?)Owner.CreateProxy(Target.Current);
			set => SetValue(x => x.Current, value?.Target);
		}

		public IBorderedBaseProxy<IBordered>? Next
		{
			get => (IBorderedBaseProxy<IBordered>?)Owner.CreateProxy(Target.Next);
			set => SetValue(x => x.Next, value?.Target);
		}

		public bool IsMove { get => Target.IsMove; set => SetValue(x => x.IsMove, value); }
	}
}
