namespace StepFlow.Core.Components
{
	public interface IScale
	{
		float Value { get; set; }

		float Max { get; set; }

		bool Freeze { get; set; }
	}
}
