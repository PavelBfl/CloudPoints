using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public class ScaleProxy : ComponentProxy<Scale>, IScaleProxy
	{
		public ScaleProxy(PlayMaster owner, Scale target) : base(owner, target)
		{
		}

		public float Value
		{
			get => Target.Value;
			set => SetValue(x => x.Value, value);
		}

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
