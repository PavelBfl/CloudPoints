using System.Collections.Generic;
using System;

namespace StepFlow.Master.Scripts
{
	public abstract class Executor<TParameters> : Executor
		where TParameters : new()
	{
		protected Executor(PlayMaster playMaster, string name) : base(name)
		{
			PlayMaster = playMaster ?? throw new ArgumentNullException(nameof(playMaster));
		}

		public PlayMaster PlayMaster { get; }

		public override void Execute(IReadOnlyDictionary<string, object>? parameters)
		{
			if (parameters is null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			object typedParameters = new TParameters();
			foreach (var propertyInfo in typeof(TParameters).GetProperties())
			{
				propertyInfo.SetValue(typedParameters, parameters[propertyInfo.Name]);
			}

			Execute((TParameters)typedParameters);
		}

		public abstract void Execute(TParameters parameters);
	}
}
