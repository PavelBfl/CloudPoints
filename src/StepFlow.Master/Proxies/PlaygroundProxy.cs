using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Core.Border;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Collections;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	internal sealed class PlaygroundProxy : ProxyBase<Playground>, IPlaygroundProxy
	{
		public PlaygroundProxy(PlayMaster owner, Playground target) : base(owner, target)
		{
		}

		public IPlayerCharacterProxy? PlayerCharacter { get => (IPlayerCharacterProxy?)Owner.CreateProxy(Target.PlayerCharacter); set => SetValue(x => x.PlayerCharacter, ((IProxyBase<PlayerCharacter>?)value)?.Target); }

		public IList<IProxyBase<Subject>> Barriers => new ListItemsProxy<Subject, IList<Subject>, IProxyBase<Subject>>(Owner, Target.Barriers);

		public Rectangle CreateRectangle(int x, int y, int width, int height) => new Rectangle(x, y, width, height);

		public Point CreatePoint(int x, int y) => new Point(x, y);

		public IBorderedProxy CreateBordered() => (IBorderedProxy)Owner.CreateProxy(new Bordered());

		public ICellProxy CreateCell(Rectangle border)
		{
			return (ICellProxy)Owner.CreateProxy(new Cell()
			{
				Border = border,
			});
		}

		public void CreatePlayerCharacter(Rectangle bounds, float strength)
		{
			PlayerCharacter = (IPlayerCharacterProxy)Owner.CreateProxy(new PlayerCharacter(Target.Context));
			PlayerCharacter.Current = CreateCell(bounds);
			PlayerCharacter.Value = strength;
			PlayerCharacter.Max = strength;
		}

		public void CreateBarrier(Rectangle bounds, float? strength)
		{
			var barrier = (IObstructionProxy)Owner.CreateProxy(new Obstruction(Target.Context));
			barrier.Current = CreateCell(bounds);

			if (strength is { })
			{
				barrier.Value = strength.Value;
				barrier.Max = strength.Value;
			}
			else
			{
				barrier.Value = 1;
				barrier.Max = 1;
				barrier.Freeze = true;
			}

			Barriers.Add(barrier);
		}
	}
}
