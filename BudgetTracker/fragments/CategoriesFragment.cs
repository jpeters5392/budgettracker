using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

namespace BudgetTracker
{
	public class CategoriesFragment : Android.App.Fragment
	{
		private const string TAG = "CategoriesFragment";
		RecyclerView categoriesRecyclerView;
		RecyclerView.Adapter categoriesAdapter;
		RecyclerView.LayoutManager categoriesLayoutManager;
		FloatingActionButton fab;

		CategoryService categoryService;
		CategoryTypeService categoryTypeService;
		InputUtilities inputUtilities;

		public CategoriesFragment() : this(new CategoryService(), new CategoryTypeService(), new InputUtilities())
		{
		}

		public CategoriesFragment (CategoryService categoryService, CategoryTypeService categoryTypeService, InputUtilities inputUtilities)
		{
			this.categoryService = categoryService;
			this.categoryTypeService = categoryTypeService;
			this.inputUtilities = inputUtilities;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Categories, container, false);
			view.SetTag (view.Id, TAG);

			fab = view.FindViewById<FloatingActionButton> (Resource.Id.fab);
			fab.Click += AddCategory;

			categoriesRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.categoriesRecyclerView);
			categoriesLayoutManager = new LinearLayoutManager (this.Activity);
			categoriesRecyclerView.SetLayoutManager (categoriesLayoutManager);

			categoriesAdapter = new CategoriesAdapter (this.categoryService, this.categoryTypeService, this.inputUtilities);
			categoriesRecyclerView.SetAdapter (categoriesAdapter);

			return view;
		}

		public override void OnDestroyView ()
		{
			fab.Click -= AddCategory;
			base.OnDestroyView ();
		}

		public void AddCategory(object sender, EventArgs e) 
		{
			var toast = Toast.MakeText (this.Activity, "Clicked FAB", ToastLength.Short);
			toast.Show ();
		}
	}
}

