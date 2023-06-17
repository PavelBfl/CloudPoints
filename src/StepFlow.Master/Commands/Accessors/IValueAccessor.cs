namespace StepFlow.Core.Commands.Accessors
{
	public interface IValueAccessor<TTarget, TValue>
	{
		void SetValue(TTarget target, TValue value);

		TValue GetValue(TTarget target);
	}
}
