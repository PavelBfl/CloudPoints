using System.Drawing;

namespace StepFlow.Master.Scripts
{
	public sealed class PlayerCharacterCreate : Executor<PlayerCharacterCreate.Parameters>
	{
		public PlayerCharacterCreate(PlayMaster playMaster) : base(playMaster, nameof(PlayerCharacterCreate))
		{
		}

		public override void Execute(Parameters parameters)
		{
			PlayMaster.GetPlaygroundProxy().CreatePlayerCharacter(
				new Rectangle(parameters.X, parameters.Y, parameters.Width, parameters.Height),
				parameters.Strength
			);
		}

		public struct Parameters
		{
			public int X { get; set; }

			public int Y { get; set; }

			public int Width { get; set; }

			public int Height { get; set; }

			public int Strength { get; set; }
		}
	}
}
