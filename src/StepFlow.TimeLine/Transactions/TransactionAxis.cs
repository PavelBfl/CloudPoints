using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.TimeLine.Transactions
{
	public class TransactionAxis : IAxis<ICommand>
	{
		public TransactionAxis(IAxis<ICommand>? source = null)
		{
			Source = source ?? new Axis<ICommand>();
		}

		public ICommand this[int index] => Transactions[index];

		public IAxis<ICommand> Source { get; }

		private IAxis<ICommand> Transactions { get; } = new Axis<ICommand>();

		public int Count => Transactions.Count;

		public int Current => Transactions.Current;

		public ITransaction? CurrentTransaction { get; private set; }

		public ITransaction BeginTransaction()
		{
			if (CurrentTransaction is { })
			{
				throw new InvalidOperationException();
			}

			CurrentTransaction = new Transaction(this);
			return CurrentTransaction;
		}

		public void Add(ICommand command, bool isCompleted = false)
		{
			if (CurrentTransaction is null)
			{
				Transactions.Add(new CommandRange(this, Source.Current, Source.Current + 1), true);
			}

			Source.Add(command, isCompleted);
		}

		public bool Execute() => Transactions.Execute();

		public bool Revert() => Transactions.Revert();

		public IEnumerator<ICommand> GetEnumerator() => Transactions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class Transaction : ITransaction
		{
			public Transaction(TransactionAxis owner)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Begin = Owner.Source.Current;
			}

			public TransactionAxis Owner { get; }

			public int Begin { get; }

			public TransactionState State { get; private set; } = TransactionState.Accumulate;

			public void Commit()
			{
				if (State != TransactionState.Accumulate)
				{
					throw new InvalidOperationException();
				}

				State = TransactionState.Commit;

				Owner.Transactions.Add(new CommandRange(Owner, Begin, Owner.Source.Current), true);
				Owner.CurrentTransaction = null;
			}

			public void Rollback()
			{
				if (State != TransactionState.Accumulate)
				{
					throw new InvalidOperationException();
				}

				State = TransactionState.Rollback;

				while (Owner.Source.Current > Begin)
				{
					Owner.Source.Revert();
				}

				Owner.CurrentTransaction = null;
			}
		}

		private sealed class CommandRange : ICommand
		{
			public CommandRange(TransactionAxis owner, int begin, int end)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Begin = begin >= -1 ? begin : throw new ArgumentOutOfRangeException(nameof(begin));
				End = end >= -1 ? end : throw new ArgumentOutOfRangeException(nameof(end));

				if (End < Begin)
				{
					throw new InvalidOperationException();
				}
			}

			public TransactionAxis Owner { get; }

			public int Begin { get; }

			public int End { get; }

			public void Execute()
			{
				while (Owner.Source.Current < End)
				{
					Owner.Source.Execute();
				}
			}

			public void Revert()
			{
				while (Owner.Source.Current > Begin)
				{
					Owner.Source.Revert();
				}
			}
		}
	}
}
