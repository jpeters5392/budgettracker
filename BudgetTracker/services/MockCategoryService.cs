using System;
using System.Linq;
using System.Collections.Generic;
using SharedPCL;
using System.Threading.Tasks;

namespace BudgetTracker
{
	public class MockCategoryService : ICategoryService
	{
		private static List<Category> categories = null;

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

		public async Task<IEnumerable<Category>> RetrieveCategories() {
			// TODO: since I am using a static list, I need to clone it so that deletions to the consumed list do not affect this list
			return this.CloneList(categories).AsEnumerable();
		}

		public Category RetrieveCategoryByName(string name) {
			return categories.Where (x => x.Name == name).FirstOrDefault ();
		}

		public Category RetrieveCategoryById(string id)
		{
			return categories.Where(x => x.Id == id).FirstOrDefault();
		}

		public void Delete(int position) {
			categories.RemoveAt (position);
		}

		public void Delete(string id)
		{
			var deletedCategory = categories.Where(x => x.Id == id).FirstOrDefault();
			if (deletedCategory != null)
			{
				categories.Remove(deletedCategory);
			}
		}

		public void Delete(Category category)
		{
			categories.Remove(category);
		}

		public void Insert(Category category) {
			if (category.Id == Guid.Empty.ToString()) {
				category.Id = Guid.NewGuid ().ToString();
			}
			categories.Add (category);
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

		Task<Category> ICategoryService.RetrieveCategoryByName(string name)
		{
			throw new NotImplementedException();
		}

		Task<Category> ICategoryService.RetrieveCategoryById(string id)
		{
			throw new NotImplementedException();
		}

		Task ICategoryService.Delete(Category category)
		{
			throw new NotImplementedException();
		}

		Task ICategoryService.Insert(Category category)
		{
			throw new NotImplementedException();
		}
	}
}

