using System;
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
			return categories;
		}

		public void Delete(int position) {
			categories.RemoveAt (position);
		}
	}
}

