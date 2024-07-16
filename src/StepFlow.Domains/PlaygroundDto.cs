using System.Collections.Generic;
using StepFlow.Domains.Elements;

namespace StepFlow.Domains
{
	public sealed class PlaygroundDto : SubjectDto
	{
		public ICollection<MaterialDto> Items { get; } = new HashSet<MaterialDto>();
	}
}
