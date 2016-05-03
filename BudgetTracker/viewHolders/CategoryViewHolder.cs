using System;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Views;

namespace BudgetTracker
{
	public class CategoryViewHolder : RecyclerView.ViewHolder
	{
		private TextView categoryNameView;
		private TextView categoryDescriptionView;

		public CategoryViewHolder (View v) : base(v)
		{
			categoryNameView = (TextView)v.FindViewById<TextView>(Resource.Id.categoryName);
			categoryDescriptionView = (TextView)v.FindViewById<TextView>(Resource.Id.categoryDescription);
		}

		public TextView CategoryNameView { 
			get {
				return categoryNameView;
			}
		}

		public TextView CategoryDescriptionView { 
			get {
				return categoryDescriptionView;
			}
		}
	}
}

