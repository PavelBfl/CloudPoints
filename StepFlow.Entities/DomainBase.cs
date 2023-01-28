using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Numerics;

namespace StepFlow.Entities
{
	public class DomainBase
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int? Id { get; set; }
	}
}
