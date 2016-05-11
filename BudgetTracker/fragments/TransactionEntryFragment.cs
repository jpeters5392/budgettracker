using System;
using System.Linq;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using SharedPCL;
using System.Collections.Generic;
using Android.Support.V7.App;
using TinyIoC;

namespace BudgetTracker
{
	public class TransactionEntryFragment : Android.Support.V4.App.Fragment
	{
		private const string FragmentTag = "TransactionEntryFragment";
		private ITransactionService transactionService;
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
		private IList<Category> categories;
		private IList<string> categoryNames;
		private readonly ILog log;

        public TransactionEntryFragment() : this(TinyIoCContainer.Current.Resolve<ITransactionService>(), 
            TinyIoCContainer.Current.Resolve<ICategoryService>(), 
            TinyIoCContainer.Current.Resolve<InputUtilities>(), 
            TinyIoCContainer.Current.Resolve<ILog>())
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BudgetTracker.TransactionEntryFragment"/> class.
		/// </summary>
		/// <param name="transactionService">An instance of the Transaction service.</param>
		/// <param name="categoryService">An instance of the Category service.</param>
		/// <param name="inputUtilities">An instance of input utilities.</param>
		/// <param name="log">An instance of a logger.</param>
		public TransactionEntryFragment (ITransactionService transactionService, ICategoryService categoryService, InputUtilities inputUtilities, ILog log)
		{
			this.transactionService = transactionService;
			this.categoryService = categoryService;
			this.inputUtilities = inputUtilities;
			this.log = log;
		}

		#region Overrides
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
			this.categoryNames = categories.Select (x => x.Name).ToList();
			this.categoriesAdapter = new ArrayAdapter<string> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, this.categoryNames);
			this.categorySpinner.Adapter = categoriesAdapter;

			this.saveButton = view.FindViewById<Button> (Resource.Id.saveButton);

			// bind event handlers
			this.saveButton.Click += SaveTransaction;
			this.categorySpinner.Touch += OnSpinnerTouched;

			// remove the focus from the edittext
			this.transactionAmount.ClearFocus ();

            this.Activity.Title = this.Activity.GetString(Resource.String.transactionEntry);

			return view;
		}

		public async override void OnResume()
		{
			//HACK: since we are overriding base methods, we cannot change the lifecycle events to return Tasks.
			// OnResume is the last lifecycle event so we have to put our async logic here so that there is nothing
			// after this that is expecting to run.
			base.OnResume();

			try
			{
				await this.categoryService.InitializeService();
				await this.transactionService.InitializeService();

				this.categories = await this.categoryService.RetrieveCategories();
                if (this.categories.Count == 0)
                {
                    // we need to alert them and take them to the categories fragment
                    var builder = new AlertDialog.Builder(this.Activity);
                    builder.SetTitle(Resource.String.missingCategories)
                       .SetMessage(Resource.String.emptyCategories)
                       .SetPositiveButton(Resource.String.go, delegate {
                           var fragment = new CategoriesFragment();
                           this.FragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout, fragment).AddToBackStack(null).Commit();
                       })
                       .SetNegativeButton(Resource.String.cancel, delegate
                       {
                           this.saveButton.Enabled = false;
                       });

                    builder.Create().Show();
                }
                else
                {
                    this.categoryNames = categories.Select(x => x.Name).ToList();
                    this.categoriesAdapter = new ArrayAdapter<string>(this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, this.categoryNames);
                    this.categorySpinner.Adapter = categoriesAdapter;
                }

				// run this on the UI thread so that it can be updated
				//this.Activity.RunOnUiThread(() => this.categorySpinner.Adapter = categoriesAdapter);
			}
			catch (Exception ex)
			{
				this.log.Error(FragmentTag, ex, "Error retrieving categories");

				// alert the user that it failed
				var toast = Toast.MakeText(this.Activity, "Error retrieving categories", ToastLength.Long);
				toast.Show();
			}
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

			this.categoryNames = null;
			this.categories = null;

			base.OnDestroyView ();
		}
		#endregion

		/// <summary>
		/// Handles a touch event on the spinner
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnSpinnerTouched(object sender, EventArgs e)
		{
			// This is needed to remove the focus from the edit text boxes, and then focus and select the spinner
			var spinner = (Spinner)sender;
			this.inputUtilities.HideKeyboard(this.view);
			this.transactionAmount.ClearFocus();
			this.transactionVendor.ClearFocus();
			this.transactionDescription.ClearFocus();
			spinner.RequestFocusFromTouch();
			spinner.PerformClick();
		}

		/// <summary>
		/// Saves the transaction.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void SaveTransaction(object sender, EventArgs e)
		{
			// hide the keyboard
			ClearFocusAndHideKeyboard ();

			decimal amount = 0;
			var validations = new bool[] { this.ValidateVendor (), this.ValidateAmount (out amount), this.ValidateCategory() };

			if (validations.All(x => x)) {
				var selectedCategory = await this.categoryService.RetrieveCategoryByName (this.categorySpinner.SelectedItem.ToString ());
				var transaction = new Transaction ();
				transaction.CategoryId = selectedCategory.Id;
				transaction.Id = Guid.NewGuid ().ToString();
				transaction.Amount = amount;
				transaction.Vendor = this.transactionVendor.Text;
				transaction.Description = this.transactionDescription.Text;
				try
				{
					await this.transactionService.Insert(transaction);

					// alert the user that it was successful
					var toast = Toast.MakeText(this.Activity, Resource.String.TransactionSaved, ToastLength.Short);
					toast.Show();

					// reset the form
					this.ResetFields();
				}
				catch (Exception ex)
				{
					log.Error(FragmentTag, ex, "Error inserting transaction");
					// alert the user that it failed
					var toast = Toast.MakeText(this.Activity, ex.Message, ToastLength.Long);
					toast.Show();
				}
			}
            else
            {
                if (!validations[0])
                {
                    this.transactionVendor.RequestFocus();
                    this.inputUtilities.ShowKeyboard(this.transactionVendor);
                }
                else if (!validations[1])
                {
                    this.transactionAmount.RequestFocus();
                    this.inputUtilities.ShowKeyboard(this.transactionAmount);
                }
            }
		}

		/// <summary>
		/// Validates the amount.
		/// </summary>
		/// <returns>Whether the amount is valid.</returns>
		/// <param name="amount">The amount.</param>
		private bool ValidateAmount(out decimal amount) {
			amount = 0;
			if (decimal.TryParse (this.transactionAmount.Text, out amount)) {
				this.amountInputLayout.Error = string.Empty;
				this.amountInputLayout.ErrorEnabled = false;
				return true;
			} else {
				this.amountInputLayout.Error = this.Activity.GetString(Resource.String.transactionIncorrectFormat);
				this.amountInputLayout.ErrorEnabled = true;
				return false;
			}
		}

		/// <summary>
		/// Validates the vendor.
		/// </summary>
		/// <returns>Whether the vendor is valid.</returns>
		private bool ValidateVendor() {
			if (string.IsNullOrWhiteSpace(this.transactionVendor.Text)) {
				this.vendorInputLayout.Error = this.Activity.GetString(Resource.String.vendorRequired);
				this.vendorInputLayout.ErrorEnabled = true;
				return false;
			} else {
				this.vendorInputLayout.Error = string.Empty;
				this.vendorInputLayout.ErrorEnabled = false;
				return true;
			}
		}

        /// <summary>
        /// Validates the category.
        /// </summary>
        /// <returns>Whether the category is valid</returns>
        private bool ValidateCategory()
        {
            if (categorySpinner.SelectedItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Resets the fields.
        /// </summary>
        private void ResetFields() {
			this.transactionAmount.Text = "";
			this.transactionVendor.Text = "";
			this.transactionDescription.Text = "";
			this.categorySpinner.SetSelection (0);
		}

		/// <summary>
		/// Clears the transaction amount text.
		/// </summary>
		/// <param name="v">The view.</param>
		private void ClearTransactionAmountText(View v) {
			this.transactionAmount.Text = "";
		}

		/// <summary>
		/// Clears the focus and hides the keyboard.
		/// </summary>
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