using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Graphics;
using Android.Views.InputMethods;
using Android.Content;

namespace BudgetTracker
{
	public class TransactionsFragment : Android.App.Fragment
	{
		string[] categories = new string[] { "Food", "Utilities", "Vacation" };
		View view;
		Button saveButton;
		AppCompatSpinner categorySpinner;
		EditText transactionAmount;
		LinearLayout transactionLayout;

		public TransactionsFragment ()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.Transactions, container, false);

			transactionAmount = view.FindViewById<EditText> (Resource.Id.transactionAmount);
			transactionLayout = view.FindViewById<LinearLayout> (Resource.Id.transactionLayout);

			categorySpinner = view.FindViewById<AppCompatSpinner> (Resource.Id.categorySpinner);
			var categoriesAdapter = new ArrayAdapter<string> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, categories);
			categorySpinner.Adapter = categoriesAdapter;

			saveButton = view.FindViewById<Button> (Resource.Id.saveButton);
			saveButton.Click += SaveTransaction;

			// set the focus to the transaction amount and activate the keyboard
			transactionAmount.ClearFocus ();

			return view;
		}

		public override void OnDestroyView ()
		{
			if (saveButton != null) {
				saveButton.Click -= SaveTransaction;
				saveButton.Dispose ();
				saveButton = null;
			}

			if (transactionAmount != null) {
				transactionAmount.Dispose ();
				transactionAmount = null;
			}

			if (categorySpinner != null) {
				categorySpinner.Dispose ();
				categorySpinner = null;
			}

			if (transactionLayout != null) {
				transactionLayout.Dispose ();
				transactionLayout = null;
			}

			if (view != null) {
				view.Dispose ();
				view = null;
			}

			base.OnDestroyView ();
		}

		public void SaveTransaction(object sender, EventArgs e)
		{
			decimal amount = 0;
			if (decimal.TryParse (transactionAmount.Text, out amount)) {
				var toast = Toast.MakeText (this.Activity, Resource.String.TransactionSaved, ToastLength.Short);
				toast.Show ();
				transactionAmount.Text = "";
				categorySpinner.SetSelection (0);

				ClearFocusAndHideKeyboard ();
			} else {
				Snackbar.Make (transactionLayout, Resource.String.transactionIncorrectFormat, Snackbar.LengthLong)
					.SetAction(this.Activity.GetString(Resource.String.clear), (v) => { transactionAmount.Text = ""; })
					.Show();
			}
		}

		private void ClearFocusAndHideKeyboard()
		{
			InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(view.ApplicationWindowToken, HideSoftInputFlags.NotAlways);
			transactionAmount.ClearFocus ();
			transactionLayout.RequestFocus ();
		}
	}
}