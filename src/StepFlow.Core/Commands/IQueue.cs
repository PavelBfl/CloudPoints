﻿using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	public interface IQueue<T> : ICollection<ITargetingCommand<T>>
	{
		
	}
}
