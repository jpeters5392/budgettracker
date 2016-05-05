using System;
using System.Linq;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Support.Design.Widget;

namespace BudgetTracker
{
	public class CategoriesAdapter : RecyclerView.Adapter
	{
		IList<Category> items;
		InputUtilities inputUtilities;
		CategoryType[] categoryTypes;
		string[] categoryTypeNames;
		RecyclerView recyclerView;
		CategoryService dataService;

		public CategoriesAdapter (CategoryService dataService, CategoryTypeService categoryTypeService, InputUtilities inputUtilities)
		{
			this.dataService = dataService;
			this.items = this.dataService.RetrieveCategories ();
			this.categoryTypes = categoryTypeService.RetrieveCategoryTypes ();
			this.categoryTypeNames = this.categoryTypes.Select (x => Enum.GetName(typeof(CategoryType), x)).ToArray();
			this.inputUtilities = inputUtilities;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			// set the view's size, margins, paddings and layout parameters
			View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CategoryListItem, parent, false);
			CategoryViewHolder vh = new CategoryViewHolder (v, inputUtilities);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
		{
			var item = this.items [position];

			// Replace the contents of the view with that element
			var holder = viewHolder as CategoryViewHolder;
			holder.CategoryNameView.SetText(item.Name, TextView.BufferType.Normal);
			holder.CategoryTypeView.Text = Enum.GetName (typeof(CategoryType), item.CategoryType);
			holder.CategoryDescriptionView.SetText(item.Description, TextView.BufferType.Normal);
			var idTag = new IdTag () { Id = item.Id };
			holder.TagId = idTag;

			// default edit view to hidden
			holder.EditLayout.Visibility = ViewStates.Gone;

			// populate the spinner
			var categoryTypesAdapter = new ArrayAdapter<string> (holder.View.Context, Resource.Layout.support_simple_spinner_dropdown_item, this.categoryTypeNames);
			holder.EditCategoryTypeView.Adapter = categoryTypesAdapter;
		}

		public override void OnAttachedToRecyclerView (RecyclerView recyclerView)
		{
			this.recyclerView = recyclerView;
			base.OnAttachedToRecyclerView (recyclerView);
		}

		public override void OnDetachedFromRecyclerView (RecyclerView recyclerView)
		{
			this.recyclerView = null;
			base.OnDetachedFromRecyclerView (recyclerView);
		}

		public override void OnViewDetachedFromWindow (Java.Lang.Object holder)
		{
			((CategoryViewHolder)holder).ItemDeleted -= OnItemDelete;
			base.OnViewDetachedFromWindow (holder);
		}

		public override void OnViewAttachedToWindow (Java.Lang.Object holder)
		{
			((CategoryViewHolder)holder).ItemDeleted += OnItemDelete;
			base.OnViewAttachedToWindow (holder);
		}

		public override int ItemCount {
			get {
				return this.items.Count;
			}
		}

		public void OnItemDelete(object sender, ItemDeletedEventArgs e)
		{
			var item = this.items [e.AdapterPosition];
			this.items.RemoveAt (e.AdapterPosition);
			this.NotifyItemRemoved (e.AdapterPosition);

			// create a snackbar to allow them to undo the action
			var snackbar = Snackbar.Make (e.View, Resource.String.categoryDeleted, Snackbar.LengthLong);
			SnackbarCallback snackbarCallback = new SnackbarCallback ();
			snackbarCallback.DismissedAction = new Action<Snackbar, int>((sbar, reason) => {
				// this method is fired if they click the Undo button, so check the reason code
				if (reason == Snackbar.Callback.DismissEventManual ||
					reason == Snackbar.Callback.DismissEventSwipe ||
					reason == Snackbar.Callback.DismissEventTimeout ||
					reason == Snackbar.Callback.DismissEventConsecutive) {
					// permanently delete the item here
					Toast.MakeText (e.View.Context, "Item permanently deleted", ToastLength.Short).Show ();
					e = null;
					sbar = null;
				}
			});
			snackbar.SetAction (Resource.String.undo, (v) => {
				// add the item back to the list
				this.items.Insert(e.AdapterPosition, item);
				this.NotifyItemInserted(e.AdapterPosition);
				this.recyclerView.ScrollToPosition(e.AdapterPosition);
				Toast.MakeText (v.Context, "Item restored", ToastLength.Short).Show ();
				e = null;
				v = null;
				item = null;
			});
			snackbar.SetCallback (snackbarCallback);
			snackbar.Show ();

		}
	}
}

