using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace BudgetTracker.Data
{
	public interface IAzureMobileService
	{
		MobileServiceClient MobileService { get; set; }
		IMobileServiceSyncTable<Category> CategoryTable { get; set; }
		bool IsInitialized { get; set; }
		Task Initialize();
		Task<bool> SyncTable<T>(IMobileServiceSyncTable<T> syncTable, string queryId);
	}
}

