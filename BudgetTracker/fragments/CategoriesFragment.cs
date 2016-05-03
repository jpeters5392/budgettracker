using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;

namespace BudgetTracker
{
	public class CategoriesFragment : Android.App.Fragment
	{
		private const string TAG = "CategoriesFragment";
		RecyclerView categoriesRecyclerView;
		RecyclerView.Adapter categoriesAdapter;
		RecyclerView.LayoutManager categoriesLayoutManager;

		Category[] categories;

		public CategoriesFragment ()
		{
			Category food = new Category ();
			food.Name = "Food";
			food.Description = "Eating out";
			food.CategoryType = CategoryType.Expense;
			food.Id = Guid.NewGuid ();
			Category utilities = new Category ();
			utilities.Name = "Utilities";
			utilities.Description = "Cable and Internet";
			utilities.CategoryType = CategoryType.Expense;
			utilities.Id = Guid.NewGuid ();
			Category vacation = new Category ();
			vacation.Name = "Vacation";
			vacation.Description = "Travel budget";
			vacation.CategoryType = CategoryType.Expense;
			vacation.Id = Guid.NewGuid ();
			categories = new Category[] { food, utilities, vacation };
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Categories, container, false);
			view.SetTag (view.Id, TAG);

			categoriesRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.categoriesRecyclerView);
			categoriesLayoutManager = new LinearLayoutManager (this.Activity);
			categoriesRecyclerView.SetLayoutManager (categoriesLayoutManager);

			categoriesAdapter = new CategoriesAdapter (this.categories);
			categoriesRecyclerView.SetAdapter (categoriesAdapter);

			return view;
		}
	}
}

