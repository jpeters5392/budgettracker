using System;
using Java.Lang;
using Android.Widget;
using Android.App;
using Android.Views;

namespace BudgetTracker
{
	public class BudgetTabAdapter : BaseAdapter<string>
	{
		string[] titles;
		Activity activity;

		public BudgetTabAdapter (Activity activity, string[] titles) : base()
		{
			this.activity = activity;
			this.titles = titles;
		}

		public override int Count
		{
			get 
			{
				return this.titles.Length;
			}
		}

		public override string this[int position]
		{
			get
			{
				return this.titles [position];
			}
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view if one is available
			if (view == null) 
			{
				view = this.activity.LayoutInflater.Inflate (Resource.Layout.NavigationDrawerListView, null);
			}

			view.FindViewById<TextView> (Resource.Id.menuRowTextView).Text = this.titles [position];
			return view;
		}
	}
}

