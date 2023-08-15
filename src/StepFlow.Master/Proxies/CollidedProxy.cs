using System.Drawing;
using MoonSharp.Interpreter;
using StepFlow.Core;
using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class CollidedProxy : ProxyBase<Collided>
	{
		[MoonSharpHidden]
		public CollidedProxy(PlayMaster owner, Collided target) : base(owner, target)
		{
		}

		public Rectangle Size
		{
			get => Target.Border?.Border ?? Rectangle.Empty;
			set
			{
				var bordered = new Bordered();
				bordered.AddCell(value);
				Target.Border = bordered;
			}
		}

		public Point Offset { get => Target.Offset; set => SetValue(x => x.Offset, value); }
	}
}
