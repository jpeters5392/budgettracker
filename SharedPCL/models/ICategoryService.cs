using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker;

namespace SharedPCL
{
	public interface ICategoryService
	{
		Task InitializeService();
		Task<IList<Category>> RetrieveCategories();
		Task<Category> RetrieveCategoryByName(string name);
		Task<Category> RetrieveCategoryById(string id);
		Task<bool> Delete(Category category);
		Task<bool> Insert(Category category);
	}
}

