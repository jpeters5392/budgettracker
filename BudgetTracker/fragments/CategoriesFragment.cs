using System;
using Android.Views;
using Android.OS;
using Android.Widget;

namespace BudgetTracker
{
	public class CategoriesFragment : Android.App.Fragment
	{
		public CategoriesFragment ()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Categories, container, false);

			return view;
		}
	}
}

