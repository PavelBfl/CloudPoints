using System.Collections.Generic;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public class WorldProvider : Dictionary<World, WorldVm>, IWorldProvider
	{
		public WorldVm GetWorld(World world) => this[world];
	}
}
