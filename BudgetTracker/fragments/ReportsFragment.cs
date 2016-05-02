using System;
using Android.Views;
using Android.OS;
using Android.Widget;

namespace BudgetTracker
{
	public class ReportsFragment : Android.App.Fragment
	{
		public ReportsFragment ()
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Reports, container, false);

			return view;
		}
	}
}

