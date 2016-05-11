using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker;

namespace SharedPCL
{
	public interface ITransactionService
	{
		Task InitializeService();
		Task<bool> Insert(Transaction transaction);
		Task<IList<Transaction>> RetrieveTransactions();
	}
}

