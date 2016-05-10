using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
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

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BudgetTracker.Data.AzureMobileService"/> class.
		/// </summary>
		/// <param name="url">The base URL for the Azure service.</param>
		/// <param name="log">An instance of a logger.</param>
		/// <param name="connectivityPlugin">The Xamarin Connectivity plugin so that we can detect network access.</param>
		public AzureMobileService(string url, ILog log, IConnectivity connectivityPlugin)
		{
			this.url = url;
			this.log = log;
			this.connectivityPlugin = connectivityPlugin;
		}

		/// <summary>
		/// Gets or sets the mobile service client.
		/// </summary>
		/// <value>The mobile service client.</value>
		public MobileServiceClient MobileService { get; set; }

		/// <summary>
		/// Gets or sets the sync table for the category model.
		/// </summary>
		/// <value>The category sync table.</value>
		public IMobileServiceSyncTable<Category> CategoryTable { get; set; }

		/// <summary>
		/// Gets or sets the sync table for the transaction model.
		/// </summary>
		/// <value>The transaction sync table.</value>
		public IMobileServiceSyncTable<Transaction> TransactionTable { get; set; }

		/// <summary>
		/// Gets or sets whether the Azure client has been initialized.
		/// </summary>
		/// <value>Whether the client is initialized.</value>
		public bool IsInitialized { get; set; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
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

		/// <summary>
		/// Syncs the table for the given model type.
		/// </summary>
		/// <returns>A Task of type bool that indicates the success.</returns>
		/// <param name="syncTable">The sync table.</param>
		/// <param name="queryId">The query identifier.</param>
		/// <typeparam name="T">The type parameter for the type of model.</typeparam>
		public async Task<bool> SyncTable<T>(IMobileServiceSyncTable<T> syncTable, string queryId)
		{
			var isConnected = this.connectivityPlugin.IsConnected;
			//var isReachable = await this.connectivityPlugin.IsReachable(this.url);
			if (!isConnected)
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

