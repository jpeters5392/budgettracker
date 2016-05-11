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
	public class CategoriesFragment : Android.Support.V4.App.Fragment
	{
		private const string FragmentTag = "CategoriesFragment";
		private RecyclerView categoriesRecyclerView;
		private FloatingActionButton fab;

		private ICategoryService categoryService;
		private ICategoryTypeService categoryTypeService;
		private InputUtilities inputUtilities;
		private CategoriesAdapter categoriesAdapter;
		private IEnumerable<Category> categories;
		private readonly ILog log;

        public CategoriesFragment() : this(TinyIoCContainer.Current.Resolve<ICategoryService>(), 
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
        public CategoriesFragment (ICategoryService categoryService, ICategoryTypeService categoryTypeService, InputUtilities inputUtilities, ILog log)
		{
			this.categoryService = categoryService;
			this.categoryTypeService = categoryTypeService;
			this.inputUtilities = inputUtilities;
			this.log = log;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Categories, container, false);

			fab = view.FindViewById<FloatingActionButton> (Resource.Id.fab);
			fab.Click += AddCategory;

			categoriesRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.categoriesRecyclerView);
			RecyclerView.LayoutManager categoriesLayoutManager = new LinearLayoutManager (this.Activity);
			categoriesRecyclerView.SetLayoutManager (categoriesLayoutManager);

			this.categories = new List<Category>();

			this.categoriesAdapter = new CategoriesAdapter (this.categoryService, this.categoryTypeService, this.inputUtilities);
			this.categoriesAdapter.Categories = this.categories.ToList();

			categoriesRecyclerView.SetAdapter (categoriesAdapter);

            this.Activity.Title = this.Activity.GetString(Resource.String.categories);

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

				this.categories = await this.categoryService.RetrieveCategories();
				this.categoriesAdapter.Categories = this.categories.ToList();
				this.categoriesAdapter.NotifyDataSetChanged();
				//this.UpdateAdapter();
			}
			catch (Exception ex)
			{
				this.log.Error(FragmentTag, ex, "Error getting categories");

				// alert the user that it failed
				var toast = Toast.MakeText(this.Activity, "Error retrieving categories", ToastLength.Long);
				toast.Show();
			}
		}

		public override void OnDestroyView ()
		{
			if (this.fab != null) {
				this.fab.Click -= AddCategory;
				this.fab.Dispose ();
				this.fab = null;
			}

			if (this.categoriesRecyclerView != null) {
				this.categoriesRecyclerView.Dispose ();
				this.categoriesRecyclerView = null;
			}

			this.categories = null;

			if (this.categoriesAdapter != null)
			{
				this.categoriesAdapter.Dispose();
				this.categoriesAdapter = null;
			}

			base.OnDestroyView ();
		}

		/// <summary>
		/// Updates the adapter on the UI thread.
		/// </summary>
		public void UpdateAdapter()
		{
			if (this.categoriesAdapter != null)
			{
				// this must run on the UI thread in order for the data to update
				this.Activity.RunOnUiThread(() => this.categoriesAdapter.NotifyDataSetChanged());
			}	
		}

		/// <summary>
		/// The event handler for clicking on the floating action button.  This will display a form to add a new category.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void AddCategory(object sender, EventArgs e) 
		{
			var toast = Toast.MakeText (this.Activity, "Clicked FAB", ToastLength.Short);
			toast.Show ();
		}
	}
}