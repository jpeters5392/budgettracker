using System;
using System.Collections.Generic;

namespace BudgetTracker
{
	public class TransactionService
	{
		private static IList<Transaction> transactions = null;

		public TransactionService ()
		{
			if (transactions == null) {
				transactions = new List<Transaction> ();
			}
		}

		public void Insert(Transaction transaction) {
			if (transaction.Id == Guid.Empty) {
				transaction.Id = Guid.NewGuid ();
			}

			transactions.Add (transaction);
		}

		public IList<Transaction> RetrieveTransactions() {
			return transactions;
		}
	}
}

