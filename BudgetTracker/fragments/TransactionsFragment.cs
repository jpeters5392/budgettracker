using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;

namespace BudgetTracker
{
	public class TransactionsFragment : Android.App.Fragment
	{
		string[] categories = new string[] { "Food", "Utilities", "Vacation" };
		View view;
		Button saveButton;
		AppCompatSpinner categorySpinner;
		EditText transactionAmount;
		public TransactionsFragment ()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.Transactions, container, false);

			transactionAmount = view.FindViewById<EditText> (Resource.Id.transactionAmount);

			categorySpinner = view.FindViewById<AppCompatSpinner> (Resource.Id.categorySpinner);
			var categoriesAdapter = new ArrayAdapter<string> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, categories);
			categorySpinner.Adapter = categoriesAdapter;

			saveButton = view.FindViewById<Button> (Resource.Id.saveButton);
			saveButton.Click += SaveTransaction;

			return view;
		}

		public override void OnDestroyView ()
		{
			saveButton.Click -= SaveTransaction;
			saveButton.Dispose ();
			saveButton = null;
			transactionAmount.Dispose ();
			transactionAmount = null;
			categorySpinner.Dispose ();
			categorySpinner = null;
			view.Dispose ();
			view = null;
			base.OnDestroyView ();
		}

		public void SaveTransaction(object sender, EventArgs e)
		{
			var toast = Toast.MakeText (this.Activity, Resource.String.TransactionSaved, ToastLength.Short);
			toast.Show ();
			transactionAmount.Text = "0.00";
			categorySpinner.SetSelection (0);
		}
	}
}