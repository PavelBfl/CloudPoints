using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies
{
	public class ScaleProxy : ProxyBase<Scale>
	{
		public ScaleProxy(PlayMaster owner, Scale target) : base(owner, target)
		{
		}

		public float Value { get => Target.Value; set => Owner.TimeAxis.Add(Target.CreatePropertyCommand(x => x.Value, value)); }

		public float Max { get => Target.Max; set => Owner.TimeAxis.Add(Target.CreatePropertyCommand(x => x.Max, value)); }

		public ScaleState Add(float value)
		{
			var result = Target.Add(value);

			Owner.TimeAxis.Add(Target.CreatePropertyCommand(x => x.Value, Target.Value), true);

			return result;
		}
	}
}
