using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedPCL;

namespace BudgetTracker
{
	public class MockTransactionService : ITransactionService
	{
		private static IList<Transaction> transactions = null;

		public MockTransactionService ()
		{
			if (transactions == null) {
				transactions = new List<Transaction> ();
			}
		}

		public async Task InitializeService()
		{
			await Task.Run(() => { });
		}

		public async Task<bool> Insert(Transaction transaction) {
			return await Task.Run(() =>
			{
				if (transaction.Id == null)
				{
					transaction.Id = Guid.NewGuid().ToString();
				}

				transactions.Add(transaction);
				return true;
			});

		}

		public async Task<IEnumerable<Transaction>> RetrieveTransactions() {
			return await Task.Run(() => transactions);
		}
	}
}

