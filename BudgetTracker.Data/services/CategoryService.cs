using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using SharedPCL;

namespace BudgetTracker.Data
{
	public class CategoryService : ICategoryService
	{
		private readonly IAzureMobileService azureMobileService;
		private readonly ILog log;

		public CategoryService(IAzureMobileService azureMobileService, ILog log)
		{
			this.azureMobileService = azureMobileService;
			this.log = log;
		}

		public async Task Delete(Category category)
		{
			await this.azureMobileService.CategoryTable.DeleteAsync(category);

			// sync categories
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");
		}

		public async Task InitializeService()
		{
			await this.azureMobileService.Initialize();
		}

		public async Task Insert(Category category)
		{
			await this.azureMobileService.CategoryTable.InsertAsync(category);

			// sync categories
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");
		}

		public async Task<IEnumerable<Category>> RetrieveCategories()
		{
			// attempt to sync
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");

			return await this.azureMobileService.CategoryTable.OrderBy(x => x.Name).ToEnumerableAsync();
		}

		public async Task<Category> RetrieveCategoryById(string id)
		{
			// attempt to sync
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");

			return await this.azureMobileService.CategoryTable.LookupAsync(id);
		}

		public async Task<Category> RetrieveCategoryByName(string name)
		{
			// attempt to sync
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");

			var results = await this.azureMobileService.CategoryTable.Where(x => x.Name == name).ToEnumerableAsync();
			return results.FirstOrDefault();
		}
	}
}

