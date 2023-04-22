using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.View.Services;
using StepFlow.ViewModel;

namespace StepFlow.View.Controls
{
	public class HexChild : ComponentContainer
	{
		private const int HEX_VERTICES_COUNT = 6;
		private const int POINTY_SPACING = 3;
		private const int FLAT_SPACING = 2;
		private const float FLAT_ANGLE_OFFSET = 0;
		private const float POINTY_ANGLE_OFFSET = MathF.Tau / (HEX_VERTICES_COUNT * 2);
		private const int MARKED_THICKNESS = 5;
		private const int UNMARKED_THICKNESS = 1;

		public HexChild(Game game, HexGrid owner, NodeVm? source = null) : base(game)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Source = source;

			Polygon = Add(new Polygon(Game));
			MouseHandler = Add(new MouseHandler(Game));
			InnerControl = Add(new Polygon(Game));

			SourceChanged();
		}

		public HexGrid Owner { get; }

		private NodeVm? source;

		public NodeVm? Source
		{
			get => source;
			set
			{
				if (Source != value)
				{
					NotifyPropertyExtensions.TryUnsubscribe(Source, SourcePropertyChanged);

					source = value;

					NotifyPropertyExtensions.TrySubscribe(Source, SourcePropertyChanged);

					SourceChanged();
				}
			}
		}

		private Polygon Polygon { get; }

		private MouseHandler MouseHandler { get; }

		private Polygon InnerControl { get; }

		private void SourceChanged()
		{
			UpdateState();
			UpdateIsMark();

			var visible = Source is { };
			Polygon.Visible = visible;
			InnerControl.Visible = visible;
		}

		private void SourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(NodeVm.State):
					UpdateState();
					break;
				case nameof(NodeVm.IsMark):
					UpdateIsMark();
					break;
			}
		}

		private void UpdateIsMark()
		{
			if (Source is { })
			{
				InnerControl.Thickness = Source.IsMark ? MARKED_THICKNESS : UNMARKED_THICKNESS;
			}
			else
			{
				InnerControl.Thickness = 0;
			}
		}

		private void UpdateState()
		{
			if (Source is { })
			{
				if (Source.State.ContainsKey(NodeState.Current))
				{
					InnerControl.Color = Color.Yellow;
				}
				else if (Source.State.ContainsKey(NodeState.Planned))
				{
					InnerControl.Color = Color.Green;
				}
				else
				{
					InnerControl.Color = default;
				}
			}
			else
			{
				InnerControl.Color = default;
			}
		}

		private IReadOnlyVertices? vertices;
		public IReadOnlyVertices Vertices
		{
			get
			{
				if (vertices is null)
				{
					vertices = new VerticesCollection(CreateVertices(Owner.Size));

					Polygon.Vertices = vertices;
					MouseHandler.Vertices = vertices;
				}

				return vertices;
			}
		}

		private static bool IsOdd(int value) => value % 2 == 1;

		private IEnumerable<Vector2> CreateVertices(float size)
		{
			var offsetAngle = Owner.Orientation switch
			{
				HexOrientation.Flat => FLAT_ANGLE_OFFSET,
				HexOrientation.Pointy => POINTY_ANGLE_OFFSET,
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};

			var position = Source?.Position ?? System.Drawing.Point.Empty;
			var cellPosition = Owner.Orientation switch
			{
				HexOrientation.Flat => new Point(
					position.X * POINTY_SPACING,
					position.Y * FLAT_SPACING + (IsOdd(position.X) == Owner.OffsetOdd ? 1 : 0)
				),
				HexOrientation.Pointy => new Point(
					position.X * FLAT_SPACING + (IsOdd(position.Y) == Owner.OffsetOdd ? 1 : 0),
					position.Y * POINTY_SPACING
				),
				_ => throw EnumNotSupportedException.Create(Owner.Orientation),
			};

			var cellOffset = Owner.GetPosition(cellPosition);

			return Utils.GetRegularPolygon(size, HEX_VERTICES_COUNT, offsetAngle)
				.Select(x => x + cellOffset);
		}

		public void Clear()
		{
			Polygon.Vertices = null;
			MouseHandler.Vertices = null;
			vertices = null;

			InnerControl.Vertices = new VerticesCollection(CreateVertices(Owner.Size * 0.9f));
		}

		public override void Update(GameTime gameTime)
		{
			if (Source is null || Source.Owner is null)
			{
				return;
			}

			var mouseService = Game.Services.GetService<IMouseService>();

			var mouseContains = Vertices.FillContains(mouseService.Position);

			Polygon.Color = mouseContains ? Color.Blue : Color.Red;

			if (mouseContains && mouseService.LeftButtonOnPress)
			{
				var keyboard = Game.Services.GetService<IKeyboardService>();
				if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
				{
					var last = Source.CreateSimple();
					Source.Owner.Current = last;
				}
				else if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
				{
					foreach (var piece in Source.Owner.Pieces.Where(x => x.IsMark))
					{
						piece.MoveTo(Source);
					}
				}
				else
				{
					Source.Owner.Current = Source.Owner.Pieces.FirstOrDefault(x => x.Current == Source);
				}
			}

			base.Update(gameTime);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				InnerControl.Dispose();
				Source = null;
			}

			base.Dispose(disposing);
		}
	}
}
