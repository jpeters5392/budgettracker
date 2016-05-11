using System;
using System.Linq;
using System.Collections.Generic;
using SharedPCL;
using System.Threading.Tasks;

namespace BudgetTracker
{
	public class MockCategoryService : ICategoryService
	{
		private static List<Category> categories = new List<Category>();

		public MockCategoryService ()
		{
			if (categories == null) {
				Category food = new Category ();
				food.Name = "Food";
				food.Description = "Eating out";
				food.CategoryType = CategoryType.Expense;
				food.Id = Guid.NewGuid ().ToString();
				Category utilities = new Category ();
				utilities.Name = "Utilities";
				utilities.Description = "Cable and Internet";
				utilities.CategoryType = CategoryType.Expense;
				utilities.Id = Guid.NewGuid ().ToString();
				Category vacation = new Category ();
				vacation.Name = "Vacation";
				vacation.Description = "Travel budget";
				vacation.CategoryType = CategoryType.Expense;
				vacation.Id = Guid.NewGuid ().ToString();
				Category paycheck = new Category ();
				paycheck.Name = "Paycheck";
				paycheck.Description = "Salary";
				paycheck.CategoryType = CategoryType.Income;
				paycheck.Id = Guid.NewGuid ().ToString();
				Category charity = new Category ();
				charity.Name = "Donations";
				charity.Description = "Charitable Giving";
				charity.CategoryType = CategoryType.Expense;
				charity.Id = Guid.NewGuid ().ToString();
				categories = new List<Category> { food, utilities, vacation, paycheck, charity };
			}
		}

		public async Task<IList<Category>> RetrieveCategories() {
			// TODO: since I am using a static list, I need to clone it so that deletions to the consumed list do not affect this list
			return await Task.Run(() => this.CloneList(categories));
		}

		public async Task<Category> RetrieveCategoryByName(string name) {
			return await Task.Run(() => categories.Where (x => x.Name == name).FirstOrDefault ());
		}

		public async Task<Category> RetrieveCategoryById(string id)
		{
			return await Task.Run(() => categories.Where(x => x.Id == id).FirstOrDefault());
		}

		public async Task<bool> Delete(Category category)
		{
			return await Task.Run(() =>
			{
				var deletedCategory = categories.Where(x => x.Id == category.Id).FirstOrDefault();
				if (deletedCategory == null)
				{
					return true;
				}

				return categories.Remove(deletedCategory);
			});
		}

		public async Task<bool> Insert(Category category) {
			if (category.Id == null) {
				category.Id = Guid.NewGuid ().ToString();
			}
			return await Task.Run(() =>
			{
				categories.Add(category);
				return true;
			});
		}

		private IList<Category> CloneList(IList<Category> categoryList)
		{
			IList<Category> clonedList = new List<Category> ();
			if (categoryList == null) {
				return null;
			}

			foreach (var category in categoryList) {
				Category newCategory = new Category ();
				newCategory.CategoryType = category.CategoryType;
				newCategory.Description = category.Description;
				newCategory.Id = category.Id;
				newCategory.Name = category.Name;
				clonedList.Add (newCategory);
			}

			return clonedList;
		}

		public Task InitializeService()
		{
			return Task.Run(() => { });
		}
	}
}

