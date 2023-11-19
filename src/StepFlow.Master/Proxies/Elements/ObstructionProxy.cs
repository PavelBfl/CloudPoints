using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IObstructionProxy : IMaterialProxy<Obstruction>
	{
		new IScaleProxy? Strength { get; set; }
	}

	internal class ObstructionProxy : MaterialProxy<Obstruction>, IObstructionProxy
	{
		public ObstructionProxy(PlayMaster owner, Obstruction target) : base(owner, target)
		{
		}

		public new IScaleProxy? Strength { get => base.Strength; set => base.Strength = value; }

		public override void OnTick()
		{
			base.OnTick();

			if (Strength?.Value == 0)
			{
				Owner.GetPlaygroundProxy().Obstructions.Remove(this);
			}
		}
	}
}
