using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.Design.Widget;

namespace BudgetTracker
{
	public class ReportsFragment : Android.App.Fragment
	{
		private TransactionService transactionService;
		private CategoryService categoryService;

		public ReportsFragment () : this(new TransactionService(), new CategoryService())
		{
		}

		public ReportsFragment (TransactionService transactionService, CategoryService categoryService)
		{
			this.transactionService = transactionService;
			this.categoryService = categoryService;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Reports, container, false);

			var reportLayout = view.FindViewById<LinearLayout> (Resource.Id.reportLayout);

			var transactions = this.transactionService.RetrieveTransactions ();

			TextView textView = new TextView (view.Context);
			textView.Text = "Transaction count: " + transactions.Count;

			reportLayout.AddView (textView);

			return view;
		}
	}
}

