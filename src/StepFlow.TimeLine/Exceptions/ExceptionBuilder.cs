namespace StepFlow.TimeLine.Exceptions
{
	internal static class ExceptionBuilder
	{
		public static AxisException CreateExecuteCompleteCommand() => new AxisException("Execute complete command.");
	}
}
