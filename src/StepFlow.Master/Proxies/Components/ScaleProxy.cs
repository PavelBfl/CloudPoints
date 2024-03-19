using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScaleProxy : IProxyBase<Scale>
	{
		int Value { get; set; }

		int Max { get; set; }

		bool Freeze { get; set; }

		bool Add(int value);

		void SetMax() => Value = Max;

		void SetMin() => Value = 0;

		void Increment() => Add(1);

		void Decrement() => Add(-1);
	}

	internal sealed class ScaleProxy : ProxyBase<Scale>, IScaleProxy
	{
		public ScaleProxy(PlayMaster owner, Scale target) : base(owner, target)
		{
		}

		public int Value { get => Target.Value; set => SetValue(value); }

		public int Max { get => Target.Max; set => SetValue(value); }

		public bool Freeze { get => Target.Freeze; set => SetValue(value); }

		public bool Add(int value)
		{
			var oldValue = Value;

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

			return newValue != oldValue;
		}
	}
}
