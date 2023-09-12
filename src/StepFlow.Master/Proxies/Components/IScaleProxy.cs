namespace StepFlow.Master.Proxies.Components
{
	public interface IScaleProxy : IComponentProxy
	{
		float Value { get; set; }

		float Max { get; set; }

		void Add(float value);
	}
}
