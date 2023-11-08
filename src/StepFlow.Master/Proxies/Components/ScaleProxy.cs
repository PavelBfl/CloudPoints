using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScaleProxy : IProxyBase<IScale>
	{
		float Value { get; set; }

		float Max { get; set; }

		void Add(float value);
	}

	internal sealed class ScaleProxy : ProxyBase<IScale>, IScaleProxy
	{
		public ScaleProxy(PlayMaster owner, IScale target) : base(owner, target)
		{
		}

		public float Value { get => Target.Value; set => SetValue(x => x.Value, value); }

		public float Max { get => Target.Max; set => SetValue(x => x.Max, value); }

		public void Add(float value)
		{
			var newValue = Value + value;
			if (newValue > Max)
			{
				Value = Max;
			}
			else if (newValue <= 0)
			{
				Value = 0;
			}
			else
			{
				Value = newValue;
			}
		}
	}
}
