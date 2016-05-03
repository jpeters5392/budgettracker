using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace BudgetTracker
{
	public class CategoriesAdapter : RecyclerView.Adapter
	{
		Category[] items;

		public CategoriesAdapter (Category[] data)
		{
			this.items = data;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			// set the view's size, margins, paddings and layout parameters
			View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CategoryListItem, parent, false);
			CategoryViewHolder vh = new CategoryViewHolder (v);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var item = this.items [position];

			// Replace the contents of the view with that element
			var holder = viewHolder as CategoryViewHolder;
			holder.CategoryNameView.SetText(item.Name, TextView.BufferType.Normal);
			holder.CategoryDescriptionView.SetText(item.Description, TextView.BufferType.Normal);
		}

		public override int ItemCount {
			get {
				return this.items.Length;
			}
		}
	}
}

