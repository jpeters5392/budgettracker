using System;
using System.Linq;
using System.Collections.Generic;

namespace BudgetTracker
{
	public class CategoryService
	{
		private static List<Category> categories = null;

		public CategoryService ()
		{
			if (categories == null) {
				Category food = new Category ();
				food.Name = "Food";
				food.Description = "Eating out";
				food.CategoryType = CategoryType.Expense;
				food.Id = Guid.NewGuid ();
				Category utilities = new Category ();
				utilities.Name = "Utilities";
				utilities.Description = "Cable and Internet";
				utilities.CategoryType = CategoryType.Expense;
				utilities.Id = Guid.NewGuid ();
				Category vacation = new Category ();
				vacation.Name = "Vacation";
				vacation.Description = "Travel budget";
				vacation.CategoryType = CategoryType.Expense;
				vacation.Id = Guid.NewGuid ();
				Category paycheck = new Category ();
				paycheck.Name = "Paycheck";
				paycheck.Description = "Salary";
				paycheck.CategoryType = CategoryType.Income;
				paycheck.Id = Guid.NewGuid ();
				Category charity = new Category ();
				charity.Name = "Donations";
				charity.Description = "Charitable Giving";
				charity.CategoryType = CategoryType.Expense;
				charity.Id = Guid.NewGuid ();
				categories = new List<Category> { food, utilities, vacation, paycheck, charity };
			}
		}

		public IList<Category> RetrieveCategories() {
			// TODO: since I am using a static list, I need to clone it so that deletions to the consumed list do not affect this list
			return this.CloneList (categories);
		}

		public Category RetrieveCategoryByName(string name) {
			return categories.Where (x => x.Name == name).FirstOrDefault ();
		}

		public void Delete(int position) {
			categories.RemoveAt (position);
		}

		public void Insert(Category category) {
			if (category.Id == Guid.Empty) {
				category.Id = Guid.NewGuid ();
			}
			categories.Add (category);
		}

		public IList<Category> CloneList(IList<Category> categoryList)
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
	}
}

