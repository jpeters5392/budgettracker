using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using SharedPCL;

namespace BudgetTracker.Data
{
	/// <summary>
	/// Category service.
	/// </summary>
	public class CategoryService : ICategoryService
	{
		private readonly IAzureMobileService azureMobileService;
		private readonly ILog log;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BudgetTracker.Data.CategoryService"/> class.
		/// </summary>
		/// <param name="azureMobileService">An instance of the Azure mobile service client.</param>
		/// <param name="log">An instance of a logger.</param>
		public CategoryService(IAzureMobileService azureMobileService, ILog log)
		{
			this.azureMobileService = azureMobileService;
			this.log = log;
		}

		/// <summary>
		/// Delete the specified category.
		/// </summary>
		/// <param name="category">The category to delete.</param>
		public async Task<bool> Delete(Category category)
		{
			await this.azureMobileService.CategoryTable.DeleteAsync(category);

			// sync categories
			return await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");
		}

		/// <summary>
		/// Initializes the service.
		/// </summary>
		public async Task InitializeService()
		{
			await this.azureMobileService.Initialize();
		}

		/// <summary>
		/// Insert the specified category.
		/// </summary>
		/// <param name="category">Category.</param>
		public async Task<bool> Insert(Category category)
		{
			await this.azureMobileService.CategoryTable.InsertAsync(category);

			// sync categories
			return await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");
		}

		/// <summary>
		/// Retrieves all of the categories.
		/// </summary>
		/// <returns>All of the categories.</returns>
		public async Task<IEnumerable<Category>> RetrieveCategories()
		{
			// attempt to sync
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");

			return await this.azureMobileService.CategoryTable.OrderBy(x => x.Name).ToEnumerableAsync();
		}

		/// <summary>
		/// Retrieves the category by identifier.
		/// </summary>
		/// <returns>The category by identifier.</returns>
		/// <param name="id">The identifier.</param>
		public async Task<Category> RetrieveCategoryById(string id)
		{
			// attempt to sync
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");

			return await this.azureMobileService.CategoryTable.LookupAsync(id);
		}

		/// <summary>
		/// Retrieves the category by name.
		/// </summary>
		/// <returns>The category by name.</returns>
		/// <param name="name">The name of the category.</param>
		public async Task<Category> RetrieveCategoryByName(string name)
		{
			// attempt to sync
			await this.azureMobileService.SyncTable<Category>(this.azureMobileService.CategoryTable, "allCategories");

			var results = await this.azureMobileService.CategoryTable.Where(x => x.Name == name).ToEnumerableAsync();
			return results.FirstOrDefault();
		}
	}
}

