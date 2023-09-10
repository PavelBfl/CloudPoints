using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScaleProxy : IComponentProxy
	{
		float Value { get; set; }
		float Max { get; set; }
		bool RemoveIfEmpty { get; set; }
		void Add(float value);
	}

	public class ScaleProxy : ComponentProxy<Scale>, IScaleProxy
	{
		public ScaleProxy(PlayMaster owner, Scale target) : base(owner, target)
		{
		}

		public float Value { get => Target.Value; set => SetValue(x => x.Value, value); }

		public float Max { get => Target.Max; set => SetValue(x => x.Max, value); }

		public bool RemoveIfEmpty { get => Target.RemoveIfEmpty; set => SetValue(x => x.RemoveIfEmpty, value); }

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

				if (RemoveIfEmpty)
				{
					var playground = (PlaygroundProxy)Owner.CreateProxy(Owner.Playground);
					var subject = (SubjectProxy)Owner.CreateProxy(Target.Container);
					playground.Subjects.Remove(subject);
				}
			}
			else
			{
				Value = newValue;
			}
		}
	}
}
