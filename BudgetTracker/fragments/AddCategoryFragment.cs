using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using SharedPCL;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TinyIoC;
using SharedPCL.models;

namespace BudgetTracker
{
    /// <summary>
    /// Categories fragment.
    /// </summary>
    public class AddCategoryFragment : Android.Support.V4.App.Fragment
    {
        private const string FragmentTag = "AddCategoryFragment";

        private ICategoryService categoryService;
        private ICategoryTypeService categoryTypeService;
        private InputUtilities inputUtilities;
        private readonly ILog log;

		private Button saveButton;
		private AppCompatSpinner categoryTypeSpinner;
		private TextInputEditText editDescription;
		private TextInputEditText editName;
		private TextInputLayout descriptionLayout;
		private TextInputLayout nameLayout;
		private IList<string> categoryTypeNames;

		public AddCategoryFragment() : this(TinyIoCContainer.Current.Resolve<ICategoryService>(),
            TinyIoCContainer.Current.Resolve<ICategoryTypeService>(),
            TinyIoCContainer.Current.Resolve<InputUtilities>(),
            TinyIoCContainer.Current.Resolve<ILog>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BudgetTracker.CategoriesFragment"/> class.
        /// </summary>
        /// <param name="categoryService">An instance of the category service.</param>
        /// <param name="categoryTypeService">An instance of the category type service.</param>
        /// <param name="inputUtilities">An instance of input utilities.</param>
        /// <param name="log">An instance of a logger.</param>
        public AddCategoryFragment(ICategoryService categoryService, ICategoryTypeService categoryTypeService, InputUtilities inputUtilities, ILog log)
        {
            this.categoryService = categoryService;
            this.categoryTypeService = categoryTypeService;
            this.inputUtilities = inputUtilities;
            this.log = log;
        }

		#region Overrides
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.AddCategory, container, false);

			this.saveButton = view.FindViewById<Button>(Resource.Id.saveButton);
			this.categoryTypeSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.categoryTypeSpinner);
			this.editDescription = view.FindViewById<TextInputEditText>(Resource.Id.description);
			this.editName = view.FindViewById<TextInputEditText>(Resource.Id.categoryName);
			this.nameLayout = view.FindViewById<TextInputLayout>(Resource.Id.categoryNameLayout);
			this.descriptionLayout = view.FindViewById<TextInputLayout>(Resource.Id.categoryDescriptionLayout);

			this.categoryTypeSpinner.Touch += OnSpinnerTouched;
			this.saveButton.Click += OnSave;

			var categoryTypes = this.categoryTypeService.RetrieveCategoryTypes();
			this.categoryTypeNames = categoryTypes.Select(x => Enum.GetName(typeof(CategoryType), x)).ToList();
			var categoryTypeAdapter = new ArrayAdapter<string>(this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, this.categoryTypeNames);
			this.categoryTypeSpinner.Adapter = categoryTypeAdapter;

			this.Activity.Title = this.Activity.GetString(Resource.String.addCategory);

            return view;
        }

        public override void OnDestroyView()
        {
			if (this.categoryTypeSpinner != null)
			{
				this.categoryTypeSpinner.Touch -= OnSpinnerTouched;
			}

			if (this.saveButton != null)
			{
				this.saveButton.Click -= OnSave;
			}

			this.DisposeView(this.saveButton);
			this.DisposeView(this.categoryTypeSpinner);
			this.DisposeView(this.editDescription);
			this.DisposeView(this.editName);
			this.DisposeView(this.nameLayout);
			this.DisposeView(this.descriptionLayout);

			base.OnDestroyView();
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
			this.inputUtilities.HideKeyboard(this.View);
			this.editDescription.ClearFocus();
			this.editName.ClearFocus();
			spinner.RequestFocusFromTouch();
			spinner.PerformClick();
		}

		protected async void OnSave(object sender, EventArgs e)
		{
			this.inputUtilities.HideKeyboard(this.View);

			var categoryName = this.editName.Text;
			var description = this.editDescription.Text;
			var categoryType = (CategoryType)Enum.Parse(typeof(CategoryType), this.categoryTypeSpinner.SelectedItem.ToString());

			if (string.IsNullOrWhiteSpace(categoryName))
			{
				this.nameLayout.Error = this.Activity.GetString(Resource.String.categoryNameRequired);
				this.nameLayout.ErrorEnabled = true;
			}
			else
			{
				this.nameLayout.Error = string.Empty;
				this.nameLayout.ErrorEnabled = false;

				var category = new Category();
				category.Name = categoryName;
				category.Description = description;
				category.CategoryType = categoryType;

				try
				{
					await this.categoryService.InitializeService();
					var success = await this.categoryService.Insert(category);

					if (success)
					{
						this.editDescription.Text = "";
						this.editName.Text = "";
						this.categoryTypeSpinner.SetSelection(0);
						this.editDescription.ClearFocus();
						this.editName.ClearFocus();
						Toast.MakeText(this.Activity, Resource.String.categorySaved, ToastLength.Long).Show();
					}
					else
					{
						Toast.MakeText(this.Activity, Resource.String.failed, ToastLength.Long).Show();
					}
				}
				catch(Exception ex)
				{
					this.log.Error(FragmentTag, ex, "Error inserting category");
					Toast.MakeText(this.Activity, Resource.String.failed, ToastLength.Long).Show();
				}
			}
		}

		private void DisposeView(View v)
		{
			if (v != null)
			{
				v.Dispose();
				v = null;
			}
		}
    }
}