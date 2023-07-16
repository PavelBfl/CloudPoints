using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Sketch
{
	public class Primitive
	{
		public Primitive(Game game)
		{
			Game = game ?? throw new ArgumentNullException(nameof(game));
			Childs = new ChildsCollection(this);
		}

		public Game Game { get; }

		private Primitive? owner;

		public Primitive? Owner
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

		public ChildsCollection Childs { get; }

		public bool Visible { get; set; } = true;

		public virtual void Draw(GameTime gameTime)
		{
		}

		public bool Enable { get; set; } = true;

		public virtual void Update(GameTime gameTime)
		{
		}

		public virtual Matrix3x2 GetMatrix() => Matrix3x2.Identity;

		public virtual RectangleF? GetPlace() => null;

		public virtual void Free()
		{
			foreach (var child in Childs)
			{
				child.Free();
			}
		}
	}

	public class Node : Primitive
	{
		public Node(Game game) : base(game)
		{
		}

		private System.Numerics.Vector2 translation;

		public System.Numerics.Vector2 Translation
		{
			get => translation;
			set => SetMatrixUnit(ref translation, value);
		}

		private System.Numerics.Vector2 scale;

		public System.Numerics.Vector2 Scale
		{
			get => scale;
			set => SetMatrixUnit(ref scale, value);
		}

		private Rotate rotate;

		public Rotate Rotate
		{
			get => rotate;
			set => SetMatrixUnit(ref rotate, value);
		}

		private void SetMatrixUnit<T>(ref T variable, T newValue)
		{
			if (!EqualityComparer<T>.Default.Equals(variable, newValue))
			{
				variable = newValue;
				matrix = null;
			}
		}

		private Matrix3x2? matrix;

		public override Matrix3x2 GetMatrix()
		{
			return matrix ??= Matrix3x2.CreateTranslation(Translation) *
					Matrix3x2.CreateScale(scale) *
					Matrix3x2.CreateRotation(Rotate.Radian);
		}
	}

	public abstract class LayoutBase : Node
	{
		protected LayoutBase(Game game) : base(game)
		{
		}

		private Thickness padding;

		public Thickness Padding { get => padding; set => SetPlaceUnit(ref padding, value); }

		private RectangleF? place;

		public override RectangleF? GetPlace() => place ??= CreatePlace();

		protected abstract RectangleF CreatePlace();

		protected void SetPlaceUnit<T>(ref T variable, T newValue)
		{
			if (!EqualityComparer<T>.Default.Equals(variable, newValue))
			{
				variable = newValue;
				place = null;
			}
		}
	}

	internal static class RectangleExtensions
	{
		public static RectangleF Offset(this RectangleF rectangle, Thickness offset) => RectangleF.FromLTRB(
			offset.Left.Kind switch
			{
				UnitKind.Pixels => rectangle.Left + offset.Left.Value,
				UnitKind.Ptc => Lerp(rectangle.Left, rectangle.Right, offset.Left.Value),
				_ => throw new InvalidOperationException(),
			},
			offset.Top.Kind switch
			{
				UnitKind.Pixels => rectangle.Top + offset.Top.Value,
				UnitKind.Ptc => Lerp(rectangle.Top, rectangle.Bottom, offset.Top.Value),
				_ => throw new InvalidOperationException(),
			},
			offset.Right.Kind switch
			{
				UnitKind.Pixels => rectangle.Right - offset.Right.Value,
				UnitKind.Ptc => Lerp(rectangle.Left, rectangle.Right, offset.Right.Value),
				_ => throw new InvalidOperationException(),
			},
			offset.Bottom.Kind switch
			{
				UnitKind.Pixels => rectangle.Bottom - offset.Bottom.Value,
				UnitKind.Ptc => Lerp(rectangle.Top, rectangle.Bottom, offset.Bottom.Value),
				_ => throw new InvalidOperationException(),
			}
		);

		private static float Lerp(float min, float max, float value) => (max - min) * value + min;
	}

	public class Canvas : LayoutBase
	{
		public Canvas(Game game) : base(game)
		{
		}

		private RectangleF bounds;

		public RectangleF Bounds { get => bounds; set => SetPlaceUnit(ref bounds, value); }

		protected override RectangleF CreatePlace() => Bounds.Offset(Padding);
	}

	public class Layout : LayoutBase
	{
		public Layout(Game game) : base(game)
		{
		}

		private RectangleF GetOwnerPlace() => Owner.GetPlace().Value;

		private Thickness margin;

		public Thickness Margin { get => margin; set => SetPlaceUnit(ref margin, value); }

		protected override RectangleF CreatePlace()
		{
			var result = GetOwnerPlace().Offset(Margin);
			result = result.Offset(Padding);
			return result;
		}
	}


	public struct Rotate : IEquatable<Rotate>
	{
		private const float FULL_DEGREE = 360;

		public Rotate CreateDegree(float value) => new Rotate() { Degree = value, };

		public Rotate CreateRadian(float value) => new Rotate() { Radian = value, };

		public float Degree { get; set; }

		public float Radian
		{
			get => MathF.Tau * Degree / FULL_DEGREE;
			set => Degree = value * FULL_DEGREE / MathF.Tau;
		}

		public readonly bool Equals(Rotate other) => Degree == other.Degree;

		public override bool Equals([NotNullWhen(true)] object? obj) => obj is Rotate other && Equals(other);

		public override int GetHashCode() => Degree.GetHashCode();
	}

	public enum UnitKind
	{
		Pixels,
		Ptc,
	}

	public struct Unit
	{
		public float Value { get; set; }

		public UnitKind Kind { get; set; }
	}

	public struct Thickness
	{
		public Unit Left { get; set; }

		public Unit Top { get; set; }

		public Unit Right { get; set; }

		public Unit Bottom { get; set; }
	}
}
