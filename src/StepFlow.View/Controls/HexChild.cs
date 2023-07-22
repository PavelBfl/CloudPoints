//using System.ComponentModel;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using StepFlow.Common.Exceptions;
//using StepFlow.View.Services;
//using StepFlow.View.Sketch;

//namespace StepFlow.View.Controls
//{
//	public class HexChild : Primitive
//	{
//		private const int POINTY_SPACING = 3;
//		private const int FLAT_SPACING = 2;
//		private const int MARKED_THICKNESS = 5;
//		private const int UNMARKED_THICKNESS = 1;

//		public HexChild(Game game) : base(game)
//		{
//			Background = new Hex(Game);
//			Selector = new Hex(Game)
//			{
//				Visible = false,
//			};

//			Childs.Add(Background);
//			Childs.Add(Selector);
//		}

//		private Hex Background { get; }

//		private Hex Selector { get; }

//		protected override void OnOwnerChange()
//		{
//			base.OnOwnerChange();
//			Refresh();
//		}

//		private static bool IsOdd(int value) => value % 2 == 1;

//		public void Refresh()
//		{
//			if (Owner is HexGrid hexGrid && Source is { })
//			{
//				Visible = true;

//				var position = Source.Position;
//				var cellPosition = hexGrid.Orientation switch
//				{
//					HexOrientation.Flat => new Point(
//						position.X * POINTY_SPACING,
//						position.Y * FLAT_SPACING + (IsOdd(position.X) == hexGrid.OffsetOdd ? 1 : 0)
//					),
//					HexOrientation.Pointy => new Point(
//						position.X * FLAT_SPACING + (IsOdd(position.Y) == hexGrid.OffsetOdd ? 1 : 0),
//						position.Y * POINTY_SPACING
//					),
//					_ => throw EnumNotSupportedException.Create(hexGrid.Orientation),
//				};

//				var center = hexGrid.GetPosition(cellPosition);

//				Background.Orientation = hexGrid.Orientation;
//				Background.Size = hexGrid.Size;
//				Background.Position = center;

//				Selector.Orientation = hexGrid.Orientation;
//				Selector.Size = hexGrid.Size * 0.9f;
//				Selector.Thickness = Source.IsMark ? MARKED_THICKNESS : UNMARKED_THICKNESS;
//				Selector.Position = center;
//				if (Source.State.ContainsKey(NodeState.Current))
//				{
//					Selector.Color = Color.Yellow;
//					Selector.Visible = true;
//				}
//				else if (Source.State.ContainsKey(NodeState.Planned))
//				{
//					Selector.Color = Color.Green;
//					Selector.Visible = true;
//				}
//				else
//				{
//					Selector.Color = default;
//					Selector.Visible = false;
//				}
//			}
//			else
//			{
//				Visible = false;
//			}
//		}

//		private void SourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
//		{
//			if (e.PropertyName is nameof(NodeVm.State) or nameof(NodeVm.IsMark))
//			{
//				Refresh();
//			}
//		}

//		public override void Update(GameTime gameTime)
//		{
//			if (Source is null || Source.Owner is null)
//			{
//				return;
//			}

//			var mouseService = Game.Services.GetService<IMouseService>();

//			var vertices = Background.GetVertices() ?? IReadOnlyVertices.Empty;
//			var mouseContains = vertices.FillContains(mouseService.Position);

//			Background.Color = mouseContains ? Color.Blue : Color.Red;

//			if (mouseContains && mouseService.LeftButtonOnPress)
//			{
//				var keyboard = Game.Services.GetService<IKeyboardService>();
//				if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
//				{
//					Source.CreateSimple();
//				}
//				else if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
//				{
//					foreach (var piece in Source.Owner.Pieces.Where(x => x.IsMark))
//					{
//						piece.MoveTo(Source);
//					}
//				}
//				else
//				{
//					Source.Owner.Current = Source.Owner.Pieces.FirstOrDefault(x => x.Current == Source);
//				}
//			}

//			base.Update(gameTime);
//		}

//		public override void Free()
//		{
//			Source = null;
//			base.Free();
//		}
//	}
//}
