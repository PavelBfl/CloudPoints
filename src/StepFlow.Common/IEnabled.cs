namespace StepFlow.Common
{
	public interface IEnabled
	{
		bool IsEnable { get; }

		void Enable();

		void Disable();
	}
}
