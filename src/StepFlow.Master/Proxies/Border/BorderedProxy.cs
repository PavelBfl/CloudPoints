using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core.Border;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Border
{
	public interface IBorderedProxy : IBorderedBaseProxy<Bordered>
	{
		new ICollection<IBorderedBaseProxy<IBordered>> Childs { get; }

		void AddCell(Rectangle rectangle);
	}

	internal sealed class BorderedProxy : BorderedBaseProxy<Bordered>, IBorderedProxy
	{
		public BorderedProxy(PlayMaster owner, Bordered target) : base(owner, target)
		{
		}

		public ICollection<IBorderedBaseProxy<IBordered>> Childs => new CollectionItemsProxy<IBordered, ICollection<IBordered>, IBorderedBaseProxy<IBordered>>(Owner, Target.Childs);

		IEnumerable<IBorderedBaseProxy<IBordered>>? IBorderedBaseProxy<Bordered>.Childs => Childs;

		public void AddCell(Rectangle rectangle)
		{
			var cell = (ICellProxy)Owner.CreateProxy(new Cell());
			cell.Border = rectangle;
			Childs.Add(cell);
		}

		public override void Offset(Point value)
		{
			foreach (var child in Childs)
			{
				child.Offset(value);
			}
		}
	}
}
