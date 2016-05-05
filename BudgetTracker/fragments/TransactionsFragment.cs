using System;
using System.Linq;
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
		CategoryService categoryService;
		View view;
		Button saveButton;
		AppCompatSpinner categorySpinner;
		EditText transactionAmount;
		LinearLayout transactionLayout;
		InputUtilities inputUtilities;

		public TransactionsFragment() : this(new CategoryService(), new InputUtilities())
		{
		}

		public TransactionsFragment (CategoryService categoryService, InputUtilities inputUtilities)
		{
			this.categoryService = categoryService;
			this.inputUtilities = inputUtilities;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.Transactions, container, false);

			transactionAmount = view.FindViewById<EditText> (Resource.Id.transactionAmount);
			transactionLayout = view.FindViewById<LinearLayout> (Resource.Id.transactionLayout);

			this.categorySpinner = view.FindViewById<AppCompatSpinner> (Resource.Id.categorySpinner);
			var categories = this.categoryService.RetrieveCategories ();
			var categoryNames = categories.Select (x => x.Name).ToArray();
			var categoriesAdapter = new ArrayAdapter<string> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, categoryNames);
			this.categorySpinner.Adapter = categoriesAdapter;

			saveButton = view.FindViewById<Button> (Resource.Id.saveButton);

			// bind event handlers
			saveButton.Click += SaveTransaction;
			this.categorySpinner.Touch += OnSpinnerTouched;

			// set the focus to the transaction amount and activate the keyboard
			transactionAmount.ClearFocus ();

			return view;
		}

		protected void OnSpinnerTouched(object sender, EventArgs e)
		{
			// This is needed to remove the focus from the edit text boxes, and then focus and select the spinner
			Spinner spinner = (Spinner)sender;
			this.transactionAmount.ClearFocus ();
			this.inputUtilities.HideKeyboard (this.view);
			spinner.RequestFocusFromTouch ();
			spinner.PerformClick ();
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

			if (this.categorySpinner != null) {
				this.categorySpinner.Touch -= OnSpinnerTouched;
				this.categorySpinner.Dispose ();
				this.categorySpinner = null;
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
			this.inputUtilities.HideKeyboard (this.view);
			transactionAmount.ClearFocus ();
			transactionLayout.RequestFocus ();
		}
	}
}