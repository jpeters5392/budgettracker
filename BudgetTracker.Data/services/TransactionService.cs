using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedPCL;

namespace BudgetTracker.Data
{
	public class TransactionService : ITransactionService
	{
		private readonly IAzureMobileService azureMobileService;
		private readonly ILog log;

		public TransactionService(IAzureMobileService azureMobileService, ILog log)
		{
			this.azureMobileService = azureMobileService;
			this.log = log;
		}

		public async Task InitializeService()
		{
			await this.azureMobileService.Initialize();
		}

		public async Task<bool> Insert(Transaction transaction)
		{
			await this.azureMobileService.TransactionTable.InsertAsync(transaction);

			// sync categories
			return await this.azureMobileService.SyncTable<Transaction>(this.azureMobileService.TransactionTable, "allTransactions");
		}

		public async Task<IList<Transaction>> RetrieveTransactions()
		{
			// attempt to sync
			await this.azureMobileService.SyncTable<Transaction>(this.azureMobileService.TransactionTable, "allTransactions");

			return await this.azureMobileService.TransactionTable.OrderBy(x => x.Vendor).ToListAsync();
		}
	}
}

