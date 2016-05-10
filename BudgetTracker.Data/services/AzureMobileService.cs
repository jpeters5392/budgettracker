using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SharedPCL;

namespace BudgetTracker.Data
{
	public class AzureMobileService : IAzureMobileService
	{
		private const string tag = "BudgetTracker.Data.AzureMobileService";
		private string url = null;
		private const string syncStorePath = "syncstore.db";
		private readonly ILog log;
		private readonly IConnectivity connectivityPlugin;

		public AzureMobileService(string url, ILog log, IConnectivity connectivityPlugin)
		{
			this.url = url;
			this.log = log;
			this.connectivityPlugin = connectivityPlugin;
		}

		public MobileServiceClient MobileService { get; set; }

		public IMobileServiceSyncTable<Category> CategoryTable { get; set; }
		public IMobileServiceSyncTable<Transaction> TransactionTable { get; set; }

		public bool IsInitialized { get; set; }

		public async Task Initialize()
		{
			if (IsInitialized)
			{
				return;
			}

			MobileService = new MobileServiceClient(this.url);

			// creates the local sqlite store for offline syncing
			var store = new MobileServiceSQLiteStore(syncStorePath);
			store.DefineTable<Category>();
			store.DefineTable<Transaction>();

			await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

			this.CategoryTable = MobileService.GetSyncTable<Category>();
			this.TransactionTable = MobileService.GetSyncTable<Transaction>();

			IsInitialized = true;
		}

		public async Task<bool> SyncTable<T>(IMobileServiceSyncTable<T> syncTable, string queryId)
		{
			var isReachable = await this.connectivityPlugin.IsReachable(this.url);
			if (!isReachable)
			{
				this.log.Debug(tag, "Cannot sync due to no connectivity");
				return false;
			}
			try
			{
				await syncTable.PullAsync(queryId, syncTable.CreateQuery());
				await MobileService.SyncContext.PushAsync();
			}
			catch (Exception ex)
			{
				this.log.Error(tag, ex, "Error syncing one of the sync tables");
				return false;
			}
			return true;
		}
	}
}

