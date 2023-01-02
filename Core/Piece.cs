using System.Windows.Input;

namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{
		}

		private HexNode? current;
		public HexNode? Current
		{
			get => current;
			set
			{
				if (Current != value)
				{
					if (Current is { })
					{
						Current.IsOccupied = false;
					}

					current = value;

					if (Current is { })
					{
						Current.IsOccupied = true;
					}
				}
			}
		}
	}

	public interface ICommandProvider
	{
		ICommand Create(string commandName);
	}
}
