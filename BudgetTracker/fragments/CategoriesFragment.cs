using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using SharedPCL;
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
		private RecyclerView.Adapter categoriesAdapter;
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

			this.categoriesAdapter = new CategoriesAdapter (categories, this.categoryService, this.categoryTypeService, this.inputUtilities);
			categoriesRecyclerView.SetAdapter (categoriesAdapter);

			return view;
		}

		public async override void OnResume()
		{
			base.OnResume();

			await this.categoryService.InitializeService();

			this.categories = await this.categoryService.RetrieveCategories();
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

			base.OnDestroyView ();
		}

		public void UpdateAdapter()
		{
			if (this.categoriesAdapter != null)
			{
				this.categoriesAdapter.NotifyDataSetChanged();
			}	
		}

		public void AddCategory(object sender, EventArgs e) 
		{
			var toast = Toast.MakeText (this.Activity, "Clicked FAB", ToastLength.Short);
			toast.Show ();
		}
	}
}