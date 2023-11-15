using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{

	public sealed class PlayerCharacter : Material
	{
		public PlayerCharacter(Context context) : base(context)
		{
		}

		public IScale? Cooldown { get; set; }
	}
}
