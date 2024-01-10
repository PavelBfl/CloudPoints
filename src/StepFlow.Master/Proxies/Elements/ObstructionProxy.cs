using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IObstructionProxy : IMaterialProxy<Obstruction>
	{
		new Scale? Strength { get; set; }
	}

	internal class ObstructionProxy : MaterialProxy<Obstruction>, IObstructionProxy
	{
		public ObstructionProxy(PlayMaster owner, Obstruction target) : base(owner, target)
		{
		}

		public new Scale? Strength { get => base.Strength; set => base.Strength = value; }

		public override void OnTick()
		{
			base.OnTick();

			if (Strength?.Value == 0)
			{
				Owner.GetPlaygroundProxy().Obstructions.Remove(this);
				Body.Current = null;
				Body.Next = null;
			}
		}
	}
}
