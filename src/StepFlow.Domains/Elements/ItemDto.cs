using System.Collections.Generic;
using StepFlow.Domains.States;

namespace StepFlow.Domains.Elements
{
	public sealed class ItemDto : MaterialDto
	{
		public ItemKind Kind { get; set; }

		public IList<ProjectileDto> Projectiles { get; } = new List<ProjectileDto>();

		public IList<StateDto> StatesSettings { get; } = new List<StateDto>();

		public int AttackCooldown { get; set; }

		public int AddStrength { get; set; }
	}
}
