﻿using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies
{
	public interface IPlaygroundProxy
	{
		IPlayerCharacterProxy? PlayerCharacter { get; set; }
		IList<IProxyBase<Subject>> Barriers { get; }

		void CreateBarrier(Rectangle bounds, float? strength);
		IBorderedProxy CreateBordered();
		ICellProxy CreateCell(Rectangle border);
		void CreatePlayerCharacter(Rectangle bounds, float strength);
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
	}
}
