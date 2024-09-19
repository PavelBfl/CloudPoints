using System.Collections.Generic;

namespace StepFlow.Domains.Elements
{
	public sealed class ProjectileDto : MaterialDto
	{
		public Damage Damage { get; set; }

		public ReusableKind Reusable { get; set; }

		public ICollection<SubjectDto> Immunity { get; } = new HashSet<SubjectDto>();
	}
}
