using System;

namespace BudgetTracker
{
	public class Transaction
	{
		public Transaction ()
		{
		}

		public Guid Id {
			get;
			set;
		}

		public decimal Amount {
			get;
			set;
		}

		public Guid CategoryId {
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

