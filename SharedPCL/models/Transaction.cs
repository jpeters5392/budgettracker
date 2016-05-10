using System;

namespace BudgetTracker
{
	public class Transaction
	{
		public Transaction ()
		{
		}

		public string Id
		{
			get;
			set;
		}

		public decimal Amount {
			get;
			set;
		}

		public string CategoryId {
			get;
			set;
		}

		public string Vendor {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}
	}
}

