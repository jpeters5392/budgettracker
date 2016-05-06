using System;

namespace BudgetTracker
{
	public class Category
	{
		public Category ()
		{
		}

		public string Name {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		public Guid Id {
			get;
			set;
		}

		public CategoryType CategoryType {
			get;
			set;
		}
	}
}

