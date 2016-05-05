using System;

namespace BudgetTracker
{
	public class CategoryTypeService
	{
		public CategoryTypeService ()
		{
		}

		public CategoryType[] RetrieveCategoryTypes() {
			return new CategoryType[] { CategoryType.Expense, CategoryType.Income };
		}
	}
}

