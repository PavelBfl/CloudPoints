using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Layout;
using StepFlow.View.Services;
using StepFlow.View.Sketch;
using StepFlow.ViewModel;

namespace StepFlow.View.Controls
{
	public class HexGrid : Primitive
	{
		private static float BigRadiusToFlatRatio { get; } = MathF.Sqrt(3);
		private static (float Pointy, float Flat, float CellPointy, float CellFlat) GetSize(float bigRadius)
		{
			var pointy = bigRadius * 2;
			var flat = bigRadius * BigRadiusToFlatRatio;
			return (pointy, flat, pointy / 4, flat / 2);
		}

		public HexGrid(Game game, ContextVm source, RectPlot plot) : base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));

			Source = source ?? throw new ArgumentNullException(nameof(source));

			Refresh();
		}

		private ContextVm? source;

		public ContextVm? Source
		{
			get => source;
			set
			{
				if (Source != value)
				{
					NotifyPropertyExtensions.TryUnsubscribe(Source?.Place, ParticlesCollectionChanged);

					source = value;

					NotifyPropertyExtensions.TrySubscribe(Source?.Place, ParticlesCollectionChanged);
				}
			}
		}

		private void ParticlesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			var nodesVm = Source?.Place.ToArray();

			if (nodesVm is null || !nodesVm.Any())
			{
				foreach (var child in Childs)
				{
					child.Free();
				}
				Childs.Clear();
				return;
			}

			while (Childs.Count < nodesVm.Length)
			{
				Childs.Add(new HexChild(Game));
			}

			while (Childs.Count > nodesVm.Length)
			{
				var lastIndex = Childs.Count - 1;
				var lastNodeV = Childs[lastIndex];
				Childs.RemoveAt(lastIndex);
				lastNodeV.Free();
			}

			for (var i = 0; i < Childs.Count; i++)
			{
				((HexChild)Childs[i]).Source = nodesVm[i];
			}
		}

		private RectPlot Plot { get; }

		private HexOrientation orientation = HexOrientation.Flat;

		public HexOrientation Orientation
		{
			get => orientation;
			set
			{
				if (Orientation != value)
				{
					orientation = value;
					Refresh();
				}
			}
		}

		private bool offsetOdd;

		public bool OffsetOdd
		{
			get => offsetOdd;
			set
			{
				if (OffsetOdd != value)
				{
					offsetOdd = value;
					Refresh();
				}
			}
		}

		public float size = 1;

		public float Size
		{
			get => size;
			set
			{
				if (Size != value)
				{
					size = value;
					Refresh();
				}
			}
		}

		private void Refresh()
		{
			switch (Orientation)
			{
				case HexOrientation.Flat:
					(Width, Height, CellWidth, CellHeight) = GetSize(Size);
					break;
				case HexOrientation.Pointy:
					(Height, Width, CellHeight, CellWidth) = GetSize(Size);
					break;
				default: throw EnumNotSupportedException.Create(Orientation);
			}
		}

		public float Width { get; private set; }
		public float Height { get; private set; }
		public float CellWidth { get; private set; }
		public float CellHeight { get; private set; }

		internal Vector2 GetPosition(Point cellPosition) => new Vector2(
			cellPosition.X * CellWidth + Plot.Bounds.Left + Width / 2,
			cellPosition.Y * CellHeight + Plot.Bounds.Top + Height / 2
		);

		public override void Update(GameTime gameTime)
		{
			var keyboardService = Game.Services.GetService<IKeyboardService>();

			if (keyboardService.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Space))
			{
				if (Source is null)
				{
					throw new PropertyNullException(nameof(Source));
				}

				Source.TakeStep();
			}

			base.Update(gameTime);
		}
	}
}
