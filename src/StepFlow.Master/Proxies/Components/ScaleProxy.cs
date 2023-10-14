using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	internal class ScaleProxy : ComponentProxy<Scale>, IScaleProxy
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
				foreach (var handler in ValueChange.Cast<IHandlerProxy>())
				{
					handler.Handle(this);
				}
			}
		}

		public float Max { get => Target.Max; set => SetValue(x => x.Max, value); }

		public ICollection<IHandlerProxy> ValueChange => CreateEvenProxy(Target.ValueChange);

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
