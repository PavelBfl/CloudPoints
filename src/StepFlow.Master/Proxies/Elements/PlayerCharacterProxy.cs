using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlayerCharacterProxy : IProxyBase<PlayerCharacter>, ICollidedProxy, IScheduledProxy, IScaleProxy
	{
		
	}

	internal class PlayerCharacterProxy : ProxyBase<PlayerCharacter>, IPlayerCharacterProxy
	{
		public PlayerCharacterProxy(PlayMaster owner, PlayerCharacter target) : base(owner, target)
		{
			collidedProxy = new CollidedProxy(owner, target);
			scheduledProxy = new ScheduledProxy(owner, target);
			scaleProxy = new ScaleProxy(owner, target);
		}

		private ICollidedProxy collidedProxy;

		public IBorderedProxy? Current { get => collidedProxy.Current; set => collidedProxy.Current = value; }

		public IBorderedProxy? Next { get => collidedProxy.Next; set => collidedProxy.Next = value; }

		public bool IsMove { get => collidedProxy.IsMove; set => collidedProxy.IsMove = value; }

		private IScheduledProxy scheduledProxy;

		public long QueueBegin { get => scheduledProxy.QueueBegin; set => scheduledProxy.QueueBegin = value; }

		public IList<Turn> Queue => scheduledProxy.Queue;

		private IScaleProxy scaleProxy;

		public float Value { get => scaleProxy.Value; set => scaleProxy.Value = value; }

		public float Max { get => scaleProxy.Max; set => scaleProxy.Max = value; }

		public void Add(float value)
		{
			var newValue = Value + value;

			if (newValue < 0)
			{
				Value = 0;
			}
			else if (newValue > Max)
			{
				Value = Max;
			}
			else
			{
				Value = newValue;
			}
		}

		ICollided IProxyBase<ICollided>.Target => Target;

		IScheduled IProxyBase<IScheduled>.Target => Target;

		IScale IProxyBase<IScale>.Target => Target;
	}
}
