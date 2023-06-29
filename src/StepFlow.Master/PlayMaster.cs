using System;
using System.ComponentModel;
using System.Drawing;
using AdaptiveExpressions;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		public long Time { get; private set; }

		public Axis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public Playground Playground { get; } = new Playground();

		public static Point CreatePoint(int x, int y) => new Point(x, y);

		public Piece CreatePiece() => new Piece(Playground);

		public Node CreateNode(Point position) => new Node(Playground, position);

		public void PiecesAdd(Piece piece) => TimeAxis.Add(new CollectionAdd<Piece>(Playground.Pieces, piece));

		public void PlaceAdd(Node node) => TimeAxis.Add(new PlaceAdd(Playground.Place, node));

		public IComponent CreateComponent(string componentName)
		{
			return componentName switch
			{
				"Scheduled" => new Scheduled(),
				"Strength" => new Scale(),
				_ => throw new InvalidOperationException(),
			};
		}

		public void AddComponent(Container container, string componentName)
		{
			TimeAxis.Add(new AddComponent(
				container,
				componentName switch
				{
					"Scheduled" => new Scheduled(),
					"Strength" => new Scale(),
					_ => throw new InvalidOperationException(),
				},
				componentName
			));
		}

		public void RemoveComponent(Container container, string componentName)
		{
			TimeAxis.Add(new Reverse(new AddComponent(
				container,
				container.Components[componentName],
				componentName
			)));
		}

		public IComponent GetComponent(Container container, string componentName) => container.Components[componentName];

		public void Execute(string expression)
		{
			if (expression is null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			var e = Expression.Parse(expression, type => new ExpressionEvaluator(type, null));

			switch (e.Type)
			{
				case nameof(PiecesAdd):
					PiecesAdd((Piece)GetValueRequired(e.Children[0]));
					break;
				case nameof(PlaceAdd):
					PlaceAdd((Node)GetValueRequired(e.Children[0]));
					break;
				case nameof(AddComponent):
					AddComponent((Container)GetValueRequired(e.Children[0]), (string)GetValueRequired(e.Children[1]));
					break;
			}
		}

		private object GetValueRequired(Expression expression) => GetValue(expression) ?? throw new InvalidOperationException();

		private object? GetValue(Expression expression)
		{
			switch (expression.Type)
			{
				case ExpressionType.Constant:
					return ((Constant)expression).Value;
				case ExpressionType.Accessor:
					var name = (string)((Constant)expression.Children[0]).Value;
					var target = expression.Children.Length > 1 ? GetValue(expression.Children[1]) : this;
					if (target is { })
					{
						var propertyInfo = target.GetType().GetProperty(name);
						return propertyInfo.GetValue(target);
					}
					else
					{
						return null;
					}
				case ExpressionType.Element:
					dynamic target0 = GetValueRequired(expression.Children[0]);
					var index = GetValue(expression.Children[1]);
					return target0[index];
				case nameof(CreatePoint):
					return CreatePoint((int)GetValueRequired(expression.Children[0]), (int)GetValueRequired(expression.Children[1]));
				case nameof(CreatePiece):
					return CreatePiece();
				case nameof(CreateNode):
					return CreateNode((Point)GetValueRequired(expression.Children[0]));
				default: throw new InvalidOperationException();
			}
		}

		public void TakeStep()
		{
			//PushToAxis(Scheduler.Queue, Time);

			//foreach (var node in Place.Values)
			//{
			//	PushToAxis(node.Scheduler.Queue, Time);
			//}

			//foreach (var piece in Pieces)
			//{
			//	PushToAxis(piece.Scheduler.Queue, Time);
			//}

			//var collision = GetCollision();

			//foreach (var collisionUnit in collision)
			//{
			//	var fullDamage = collisionUnit.Sum(x => x.CollisionDamage);

			//	foreach (var piece in collisionUnit)
			//	{
			//		var addResult = piece.Strength.Add(-(fullDamage - piece.CollisionDamage));
			//		if (addResult == StrengthState.Min)
			//		{
			//			Pieces.Remove(piece);
			//		}
			//		else
			//		{
			//			Clear(piece);
			//		}
			//	}
			//}

			//foreach (var piece in Pieces)
			//{
			//	TakeStep(piece);
			//}

			//Time++;
		}
	}
}
