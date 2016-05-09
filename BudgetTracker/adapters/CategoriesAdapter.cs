using System;
using System.Linq;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using SharedPCL;

namespace BudgetTracker
{
	public class CategoriesAdapter : RecyclerView.Adapter
	{
		private IList<Category> categories;
		private InputUtilities inputUtilities;
		private CategoryType[] categoryTypes;
		private string[] categoryTypeNames;
		private RecyclerView recyclerView;
		private readonly ICategoryService categoryService;

		public CategoriesAdapter (ICategoryService dataService, CategoryTypeService categoryTypeService, InputUtilities inputUtilities)
		{
			this.categories = categories.ToList();
			this.categoryService = dataService;
			this.categoryTypes = categoryTypeService.RetrieveCategoryTypes ();
			this.categoryTypeNames = this.categoryTypes.Select (x => Enum.GetName(typeof(CategoryType), x)).ToArray();
			this.inputUtilities = inputUtilities;
		}

		public IList<Category> Categories
		{
			get
			{
				return this.categories;
			}
			set
			{
				this.categories = value;
			}
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
			var item = this.categories [position];

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
				return this.categories.Count;
			}
		}

		public void OnItemDelete(object sender, ItemDeletedEventArgs e)
		{
			var item = this.categories [e.AdapterPosition];
			this.categories.RemoveAt (e.AdapterPosition);
			this.NotifyItemRemoved (e.AdapterPosition);

			// create a snackbar to allow them to undo the action
			var snackbar = Snackbar.Make (e.View, Resource.String.categoryDeleted, Snackbar.LengthLong);

			// this callback isn't actually necessary for our case, but I included it to show how you can trigger events when the snackbar goes away
			var snackbarCallback = new SnackbarCallback ();
			snackbarCallback.DismissedAction = new Action<Snackbar, int>((sbar, reason) => {
				// this method is also fired if they click the Undo button, so check the reason code
				if (reason == Snackbar.Callback.DismissEventManual ||
					reason == Snackbar.Callback.DismissEventSwipe ||
					reason == Snackbar.Callback.DismissEventTimeout ||
					reason == Snackbar.Callback.DismissEventConsecutive) {

					//TODO: Delete is asynchronous, but this action is synchronous
					// permanently delete the item
					this.categoryService.Delete(item);

					// cleanup
					e = null;
					sbar = null;
				}
			});
			snackbar.SetAction (Resource.String.undo, (v) => {
				// they clicked Undo, so add the item back to the list
				this.categories.Insert(e.AdapterPosition, item);
				this.NotifyItemInserted(e.AdapterPosition);

				// scroll to the item
				this.recyclerView.ScrollToPosition(e.AdapterPosition);

				// let them know it was restored
				Toast.MakeText (v.Context, "Item restored", ToastLength.Short).Show ();

				// cleanup
				e = null;
				v = null;
				item = null;
			});
			snackbar.SetCallback (snackbarCallback);
			snackbar.Show ();

		}
	}
}

