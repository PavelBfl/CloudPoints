﻿using System.Diagnostics.CodeAnalysis;

namespace StepFlow.Entities
{
	public class PieceEntity : InheritorParticle
	{
		public int CurrentId { get; set; }

		[AllowNull]
		public HexNodeEntity Current { get; set; }
	}
}