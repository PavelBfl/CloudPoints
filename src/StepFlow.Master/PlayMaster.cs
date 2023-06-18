using System;
using AdaptiveExpressions;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.Master
{
	public class PlayMaster
	{
		public long Time { get; private set; }

		public Axis<ICommand> TimeAxis { get; } = new Axis<ICommand>();

		public Playground Playground { get; } = new Playground();

		public void Test(int i, string s)
		{
			
		}

		public void Execute(string expression)
		{
			if (expression is null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			var e = Expression.Parse(expression, type => new ExpressionEvaluator(type, null));

			switch (e.Type)
			{
				case nameof(Test):
					Test((int)GetValue(e.Children[0]), (string)GetValue(e.Children[1]));
					break;
			}
		}

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
					dynamic? target0 = GetValue(expression.Children[0]);
					var index = GetValue(expression.Children[1]);
					return target0[index];
				default: throw new InvalidOperationException();
			}
		}

		public void TakeStep()
		{
			PushToAxis(Scheduler.Queue, Time);

			foreach (var node in Place.Values)
			{
				PushToAxis(node.Scheduler.Queue, Time);
			}

			foreach (var piece in Pieces)
			{
				PushToAxis(piece.Scheduler.Queue, Time);
			}

			var collision = GetCollision();

			foreach (var collisionUnit in collision)
			{
				var fullDamage = collisionUnit.Sum(x => x.CollisionDamage);

				foreach (var piece in collisionUnit)
				{
					var addResult = piece.Strength.Add(-(fullDamage - piece.CollisionDamage));
					if (addResult == StrengthState.Min)
					{
						Pieces.Remove(piece);
					}
					else
					{
						Clear(piece);
					}
				}
			}

			foreach (var piece in Pieces)
			{
				TakeStep(piece);
			}

			Time++;
		}
	}
}
