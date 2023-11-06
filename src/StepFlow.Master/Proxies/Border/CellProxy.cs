using System.Drawing;
using StepFlow.Core.Border;

namespace StepFlow.Master.Proxies.Border
{
	public interface ICellProxy : IBorderedBaseProxy<Cell>
	{
		new Rectangle Border { get; set; }
	}

	internal sealed class CellProxy : BorderedBaseProxy<Cell>, ICellProxy
	{
		public CellProxy(PlayMaster owner, Cell target) : base(owner, target)
		{
		}

		public Rectangle Border { get => Target.Border; set => SetValue(x => x.Border, value); }

		public override void Offset(Point value)
		{
			var newValue = Border;
			newValue.Offset(value);
			Border = newValue;
		}
	}
}
