using System.Linq;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.Design.Widget;
using SharedPCL;
using System;

namespace BudgetTracker
{
	public class ReportsFragment : Android.App.Fragment
	{
		private const string Tag = "ReportsFragment";
		private ITransactionService transactionService;
		private ICategoryService categoryService;
		private View view;
		private LinearLayout reportLayout;
		private readonly ILog log;

		public ReportsFragment (ITransactionService transactionService, ICategoryService categoryService, ILog log)
		{
			this.transactionService = transactionService;
			this.categoryService = categoryService;
			this.log = log;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			this.view = inflater.Inflate (Resource.Layout.Reports, container, false);

			this.reportLayout = this.view.FindViewById<LinearLayout> (Resource.Id.reportLayout);

			return view;
		}

		public async override void OnResume()
		{
			base.OnResume();

			try
			{
				await this.transactionService.InitializeService();

				var transactions = await this.transactionService.RetrieveTransactions();

				this.Activity.RunOnUiThread(() =>
				{
					var textView = new TextView(this.view.Context);
					textView.Text = "Transaction count: " + transactions.ToList().Count;
					this.reportLayout.AddView(textView);
				});
			}
			catch (Exception ex)
			{
				this.log.Error(Tag, ex, "Error getting transactions");
			}
		}

		public override void OnDestroy()
		{
			if (this.reportLayout != null)
			{
				this.reportLayout.Dispose();
				this.reportLayout = null;
			}

			if (this.view != null)
			{
				this.view.Dispose();
				this.view = null;
			}

			base.OnDestroy();
		}
	}
}

