using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using System.Drawing;
using System.Numerics;

namespace StepFlow.Markup;

internal class PlaygroundBuilder
{
	private static Size CellSize => new(Playground.CELL_SIZE_DEFAULT, Playground.CELL_SIZE_DEFAULT);

	private static int PlaygroundToGlobal(int value) => value * Playground.CELL_SIZE_DEFAULT;

	private static Point PlaygroundToGlobal(int x, int y) => new(PlaygroundToGlobal(x), PlaygroundToGlobal(y));

	private static Point PlaygroundToGlobal(Point position) => PlaygroundToGlobal(position.X, position.Y);

	private static Rectangle CreateCell(int x, int y) => new(PlaygroundToGlobal(x, y), CellSize);

	private static Rectangle CreateCell(Point position) => CreateCell(position.X, position.Y);

	private static Vector2 CreateRotate(float radians)
	{
		return Vector2.Transform(
			new Vector2(1, 0),
			Matrix3x2.CreateRotation(radians)
		);
	}

	public PlaygroundDto CreateState0()
	{
		return new PlaygroundDto()
		{
			Items =
			{
				new ObstructionDto()
				{
					Kind = ObstructionKind.Tiles,
					View = ObstructionView.DarkWall,
					Weight = Material.MAX_WEIGHT,
					Body = new CollidedDto()
					{
						Current = { CreateCell(0, 0) },
						IsRigid = true,
					},
				},
			}
		};
	}
}
