using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Particle
	{
		private IWorld? owner;

		public IWorld? Owner
		{
			get => owner;
			internal set
			{
				if (Owner != value)
				{
					owner = value;
					OnOwnerChange();
				}
			}
		}

		protected virtual void OnOwnerChange()
		{
		}

		public IWorld OwnerRequired => Owner ?? throw ExceptionBuilder.CreateInvalidAccessOwner();
	}
}
