using System.Linq;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.Design.Widget;
using SharedPCL;
using System;
using TinyIoC;

namespace BudgetTracker
{
	/// <summary>
	/// Reports fragment.
	/// </summary>
	public class ReportsFragment : Android.Support.V4.App.Fragment
	{
		private const string FragmentTag = "ReportsFragment";
		private ITransactionService transactionService;
		private ICategoryService categoryService;
		private View view;
		private LinearLayout reportLayout;
		private readonly ILog log;

        public ReportsFragment() : this(TinyIoCContainer.Current.Resolve<ITransactionService>(), 
            TinyIoCContainer.Current.Resolve<ICategoryService>(),
            TinyIoCContainer.Current.Resolve<ILog>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BudgetTracker.ReportsFragment"/> class.
        /// </summary>
        /// <param name="transactionService">An instance of the transaction service.</param>
        /// <param name="categoryService">An instance of the category service.</param>
        /// <param name="log">An instance of a logger.</param>
        public ReportsFragment (ITransactionService transactionService, ICategoryService categoryService, ILog log)
		{
			this.transactionService = transactionService;
			this.categoryService = categoryService;
			this.log = log;
		}

		#region Overrides
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			this.view = inflater.Inflate (Resource.Layout.Reports, container, false);

			this.reportLayout = this.view.FindViewById<LinearLayout> (Resource.Id.reportLayout);

            this.Activity.Title = this.Activity.GetString(Resource.String.reports);

            return view;
		}

		public async override void OnResume()
		{
			base.OnResume();

			try
			{
				await this.transactionService.InitializeService();

				var transactions = await this.transactionService.RetrieveTransactions();

				//this.Activity.RunOnUiThread(() =>
				//{
					var textView = new TextView(this.view.Context);
					textView.Text = "Transaction count: " + transactions.ToList().Count;
					this.reportLayout.AddView(textView);
				//});
			}
			catch (Exception ex)
			{
				this.log.Error(FragmentTag, ex, "Error getting transactions");

				// alert the user that it failed
				var toast = Toast.MakeText(this.Activity, "Error retrieving transactions", ToastLength.Long);
				toast.Show();
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
		#endregion
	}
}

