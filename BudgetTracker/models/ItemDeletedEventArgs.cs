using System;
using Android.Views;

namespace BudgetTracker
{
	public class ItemDeletedEventArgs : EventArgs
	{
		public ItemDeletedEventArgs ()
		{
		}

		public int AdapterPosition {
			get;
			set;
		}

		public View View {
			get;
			set;
		}
	}
}

