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

namespace BudgetTracker
{
	public class CategoriesFragment : Android.App.Fragment
	{
		private RecyclerView categoriesRecyclerView;
		private FloatingActionButton fab;

		private ICategoryService categoryService;
		private CategoryTypeService categoryTypeService;
		private InputUtilities inputUtilities;
		private CategoriesAdapter categoriesAdapter;
		private IEnumerable<Category> categories;

		public CategoriesFragment (ICategoryService categoryService, CategoryTypeService categoryTypeService, InputUtilities inputUtilities)
		{
			this.categoryService = categoryService;
			this.categoryTypeService = categoryTypeService;
			this.inputUtilities = inputUtilities;
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

			return view;
		}

		public async override void OnResume()
		{
			//HACK: since we are overriding base methods, we cannot change the lifecycle events to return Tasks.
			// OnResume is the last lifecycle event so we have to put our async logic here so that there is nothing
			// after this that is expecting to run.
			base.OnResume();

			await this.categoryService.InitializeService();

			this.categories = await this.categoryService.RetrieveCategories();
			this.categoriesAdapter.Categories = this.categories.ToList();
			this.UpdateAdapter();
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

		public void UpdateAdapter()
		{
			if (this.categoriesAdapter != null)
			{
				// this must run on the UI thread in order for the data to update
				this.Activity.RunOnUiThread(() => this.categoriesAdapter.NotifyDataSetChanged());
			}	
		}

		public void AddCategory(object sender, EventArgs e) 
		{
			var toast = Toast.MakeText (this.Activity, "Clicked FAB", ToastLength.Short);
			toast.Show ();
		}
	}
}