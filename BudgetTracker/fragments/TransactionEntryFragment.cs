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
using SharedPCL;
using System.Collections.Generic;

namespace BudgetTracker
{
	public class TransactionEntryFragment : Android.App.Fragment
	{
		private TransactionService transactionService;
		private ICategoryService categoryService;
		private View view;
		private Button saveButton;
		private AppCompatSpinner categorySpinner;
		private EditText transactionAmount;
		private EditText transactionVendor;
		private EditText transactionDescription;
		private LinearLayout transactionLayout;
		private InputUtilities inputUtilities;
		private TextInputLayout amountInputLayout;
		private TextInputLayout vendorInputLayout;
		private ArrayAdapter categoriesAdapter;
		private IEnumerable<Category> categories;
		private string[] categoryNames;

		public TransactionEntryFragment (TransactionService transactionService, ICategoryService categoryService, InputUtilities inputUtilities)
		{
			this.transactionService = transactionService;
			this.categoryService = categoryService;
			this.inputUtilities = inputUtilities;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			this.view = inflater.Inflate (Resource.Layout.TransactionEntry, container, false);

			this.transactionAmount = view.FindViewById<EditText> (Resource.Id.transactionAmount);
			this.transactionVendor = view.FindViewById<EditText> (Resource.Id.transactionVendor);
			this.transactionDescription = view.FindViewById<EditText> (Resource.Id.transactionDescription);
			this.transactionLayout = view.FindViewById<LinearLayout> (Resource.Id.transactionLayout);
			this.amountInputLayout = view.FindViewById<TextInputLayout> (Resource.Id.amountInputLayout);
			this.vendorInputLayout = view.FindViewById<TextInputLayout> (Resource.Id.vendorInputLayout);

			this.categorySpinner = view.FindViewById<AppCompatSpinner> (Resource.Id.categorySpinner);
			this.categories = new List<Category>();
			this.categoryNames = categories.Select (x => x.Name).ToArray();
			this.categoriesAdapter = new ArrayAdapter<string> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, this.categoryNames);
			this.categorySpinner.Adapter = categoriesAdapter;

			this.saveButton = view.FindViewById<Button> (Resource.Id.saveButton);

			// bind event handlers
			this.saveButton.Click += SaveTransaction;
			this.categorySpinner.Touch += OnSpinnerTouched;

			// remove the focus from the edittext
			this.transactionAmount.ClearFocus ();

			return view;
		}

		public async override void OnResume()
		{
			base.OnResume();

			await this.categoryService.InitializeService();

			this.categories = await this.categoryService.RetrieveCategories();
			this.categoryNames = categories.Select(x => x.Name).ToArray();

			UpdateAdapter();
		}

		public void UpdateAdapter()
		{
			if (this.categoriesAdapter != null)
			{
				this.categoriesAdapter.NotifyDataSetChanged();
			}
		}

		protected void OnSpinnerTouched(object sender, EventArgs e)
		{
			// This is needed to remove the focus from the edit text boxes, and then focus and select the spinner
			Spinner spinner = (Spinner)sender;
			this.inputUtilities.HideKeyboard (this.view);
			this.transactionAmount.ClearFocus ();
			this.transactionVendor.ClearFocus ();
			this.transactionDescription.ClearFocus ();
			spinner.RequestFocusFromTouch ();
			spinner.PerformClick ();
		}

		public override void OnDestroyView ()
		{
			if (this.saveButton != null) {
				this.saveButton.Click -= SaveTransaction;
				this.saveButton.Dispose ();
				this.saveButton = null;
			}

			if (this.transactionAmount != null) {
				this.transactionAmount.Dispose ();
				this.transactionAmount = null;
			}

			if (this.transactionVendor != null) {
				this.transactionVendor.Dispose ();
				this.transactionVendor = null;
			}

			if (this.transactionDescription != null) {
				this.transactionDescription.Dispose ();
				this.transactionDescription = null;
			}

			if (this.categorySpinner != null) {
				this.categorySpinner.Touch -= OnSpinnerTouched;
				this.categorySpinner.Dispose ();
				this.categorySpinner = null;
			}

			if (this.transactionLayout != null) {
				this.transactionLayout.Dispose ();
				this.transactionLayout = null;
			}

			if (this.view != null) {
				this.view.Dispose ();
				this.view = null;
			}

			base.OnDestroyView ();
		}

		public async void SaveTransaction(object sender, EventArgs e)
		{
			// hide the keyboard
			ClearFocusAndHideKeyboard ();

			decimal amount = 0;
			bool[] validations = new bool[] { this.ValidateVendor (), this.ValidateAmount (out amount) };

			if (validations.All(x => x)) {
				var selectedCategory = await this.categoryService.RetrieveCategoryByName (this.categorySpinner.SelectedItem.ToString ());
				Transaction transaction = new Transaction ();
				transaction.CategoryId = selectedCategory.Id;
				transaction.Id = Guid.NewGuid ();
				transaction.Amount = amount;
				transaction.Vendor = this.transactionVendor.Text;
				transaction.Description = this.transactionDescription.Text;
				this.transactionService.Insert (transaction);

				// alert the user that it was successful
				var toast = Toast.MakeText (this.Activity, Resource.String.TransactionSaved, ToastLength.Short);
				toast.Show ();

				// reset the form
				this.ResetFields();
			}
		}

		private bool ValidateAmount(out decimal amount) {
			amount = 0;
			if (decimal.TryParse (this.transactionAmount.Text, out amount)) {
				this.amountInputLayout.Error = string.Empty;
				this.amountInputLayout.ErrorEnabled = false;
				return true;
			} else {
				this.amountInputLayout.Error = this.Activity.GetString(Resource.String.transactionIncorrectFormat);
				this.amountInputLayout.ErrorEnabled = true;
				this.transactionAmount.RequestFocus ();
				this.inputUtilities.ShowKeyboard (this.transactionAmount);
				return false;
			}
		}

		private bool ValidateVendor() {
			if (string.IsNullOrWhiteSpace(this.transactionVendor.Text)) {
				this.vendorInputLayout.Error = this.Activity.GetString(Resource.String.vendorRequired);
				this.vendorInputLayout.ErrorEnabled = true;
				this.transactionVendor.RequestFocus ();
				this.inputUtilities.ShowKeyboard (this.transactionVendor);
				return false;
			} else {
				this.vendorInputLayout.Error = string.Empty;
				this.vendorInputLayout.ErrorEnabled = false;
				return true;
			}
		}

		private void ResetFields() {
			this.transactionAmount.Text = "";
			this.transactionVendor.Text = "";
			this.transactionDescription.Text = "";
			this.categorySpinner.SetSelection (0);
		}

		private void ClearTransactionAmountText(View v) {
			this.transactionAmount.Text = "";
		}

		private void ClearFocusAndHideKeyboard()
		{
			this.inputUtilities.HideKeyboard (this.view);
			this.transactionAmount.ClearFocus ();
			this.transactionVendor.ClearFocus ();
			this.transactionDescription.ClearFocus ();

			// focus on the layout
			this.transactionLayout.RequestFocus ();
		}
	}
}