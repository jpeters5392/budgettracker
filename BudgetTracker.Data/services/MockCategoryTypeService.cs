using SharedPCL.models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetTracker
{
	public class MockCategoryTypeService : ICategoryTypeService
	{
		public MockCategoryTypeService()
		{
		}

		public IList<CategoryType> RetrieveCategoryTypes() {
            return new List<CategoryType>() { CategoryType.Expense, CategoryType.Income };
		}
	}
}

