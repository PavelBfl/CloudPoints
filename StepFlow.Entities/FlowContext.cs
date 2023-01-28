using Microsoft.EntityFrameworkCore;

namespace StepFlow.Entities
{
	public class FlowContext : DbContext
	{
		private const string FILE_NAME = "Mobile.db";
		private const string CONNECTION_STRING = "Filename=" + FILE_NAME;

		public FlowContext()
		{
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(CONNECTION_STRING);
		}
	}
}
