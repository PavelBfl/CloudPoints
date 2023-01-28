using System.ComponentModel.DataAnnotations.Schema;

namespace StepFlow.Entities
{
	public class EntityBase
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int? Id { get; set; }
	}
}
