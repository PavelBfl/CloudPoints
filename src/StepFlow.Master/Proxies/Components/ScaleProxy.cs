using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Components.Custom;

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
			set
			{
				SetValue(x => x.Value, value);
				foreach (var id in ValueChange)
				{
					var handler = (IScaleHandler)Owner.Playground.Objects[id];
					handler.ValueChange(this, Target.Site.Name);
				}
			}
		}

		public float Max { get => Target.Max; set => SetValue(x => x.Max, value); }

		public ICollection<uint> ValueChange => CreateEvenProxy(Target.ValueChange);

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
