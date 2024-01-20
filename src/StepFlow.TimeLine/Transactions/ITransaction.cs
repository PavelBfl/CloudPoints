namespace StepFlow.TimeLine.Transactions
{
	public interface ITransaction
	{
		void Commit();

		void Rollback();
	}
}
