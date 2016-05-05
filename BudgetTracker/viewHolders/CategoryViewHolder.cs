using System;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Content;
using Android.Animation;
using Android.Support.Design.Widget;
using Android.Support.V7.App;

namespace BudgetTracker
{
	public class CategoryViewHolder : RecyclerView.ViewHolder
	{
		private InputUtilities inputUtilities;

		public CategoryViewHolder (View v, InputUtilities inputUtilities) : base(v)
		{
			this.View = v;
			this.inputUtilities = inputUtilities;
			this.CategoryNameView = this.View.FindViewById<TextView>(Resource.Id.categoryName);
			this.EditCategoryNameView = this.View.FindViewById<EditText>(Resource.Id.editCategoryName);
			this.EditCategoryDescriptionView = this.View.FindViewById<EditText>(Resource.Id.editCategoryDescription);
			this.EditCategoryTypeView = this.View.FindViewById<AppCompatSpinner>(Resource.Id.editCategoryType);
			this.CategoryDescriptionView = this.View.FindViewById<TextView>(Resource.Id.categoryDescription);
			this.CategoryTypeView = this.View.FindViewById<TextView>(Resource.Id.categoryType);
			this.DisplayLayout = this.View.FindViewById<LinearLayout> (Resource.Id.displayHolder);
			this.EditLayout = this.View.FindViewById<LinearLayout> (Resource.Id.editHolder);
			this.EditCategoryButton = this.View.FindViewById<Button> (Resource.Id.editCategory);
			this.SaveCategoryButton = this.View.FindViewById<Button> (Resource.Id.saveCategory);
			this.CancelCategoryButton = this.View.FindViewById<Button> (Resource.Id.cancelCategory);
			this.DeleteCategoryButton = this.View.FindViewById<Button> (Resource.Id.deleteCategory);

			// add event handlers
			this.EditCategoryButton.Click += OnEdit;
			this.SaveCategoryButton.Click += OnSave;
			this.CancelCategoryButton.Click += OnCancel;
			this.DeleteCategoryButton.Click += OnDelete;
			this.EditCategoryTypeView.Touch += OnSpinnerTouched;
		}

		public event EventHandler<ItemDeletedEventArgs> ItemDeleted;

		protected override void Dispose (bool disposing)
		{
			if (this.EditCategoryButton != null) {
				this.EditCategoryButton.Click -= OnEdit;
				this.EditCategoryButton.Dispose ();
				this.EditCategoryButton = null;
			}

			if (this.SaveCategoryButton != null) {
				this.SaveCategoryButton.Click -= OnSave;
				this.SaveCategoryButton.Dispose ();
				this.SaveCategoryButton = null;
			}

			if (this.CancelCategoryButton != null) {
				this.CancelCategoryButton.Click -= OnCancel;
				this.CancelCategoryButton.Dispose ();
				this.CancelCategoryButton = null;
			}

			if (this.DeleteCategoryButton != null) {
				this.DeleteCategoryButton.Click -= OnDelete;
				this.DeleteCategoryButton.Dispose ();
				this.DeleteCategoryButton = null;
			}

			if (this.EditCategoryTypeView != null) {
				this.EditCategoryTypeView.Touch -= OnSpinnerTouched;
				this.EditCategoryTypeView.Dispose ();
				this.EditCategoryTypeView = null;
			}

			base.Dispose (disposing);
		}

		public View View {
			get;
			set;
		}

		public TextView CategoryNameView { 
			get; 
			set;
		}

		public TextView CategoryDescriptionView { 
			get; 
			set;
		}

		public TextView CategoryTypeView { 
			get; 
			set;
		}

		public EditText EditCategoryNameView { 
			get; 
			set;
		}

		public EditText EditCategoryDescriptionView { 
			get; 
			set;
		}

		public Spinner EditCategoryTypeView { 
			get; 
			set;
		}

		public Button EditCategoryButton { 
			get; 
			set;
		}

		public Button CancelCategoryButton { 
			get; 
			set;
		}

		public Button SaveCategoryButton { 
			get; 
			set;
		}

		public Button DeleteCategoryButton { 
			get; 
			set;
		}

		public View EditLayout { 
			get; 
			set;
		}

		public View DisplayLayout { 
			get; 
			set;
		}

		public IdTag TagId
		{
			get {
				return (IdTag)this.View.GetTag (Resource.Id.view_tag_id);
			}
			set {
				this.View.SetTag (Resource.Id.view_tag_id, value);
			}
		}

		protected void OnSpinnerTouched(object sender, EventArgs e)
		{
			// This is needed to remove the focus from the edit text boxes, and then focus and select the spinner
			Spinner spinner = (Spinner)sender;
			this.EditCategoryNameView.ClearFocus ();
			this.EditCategoryDescriptionView.ClearFocus ();
			this.inputUtilities.HideKeyboard (this.View);
			spinner.RequestFocusFromTouch ();
			spinner.PerformClick ();
		}

		protected void OnEdit(object sender, EventArgs e)
		{
			this.EditCategoryNameView.Text = this.CategoryNameView.Text;
			this.EditCategoryDescriptionView.Text = this.CategoryDescriptionView.Text;
			int selectedIndex = ((ArrayAdapter<string>)this.EditCategoryTypeView.Adapter).GetPosition (this.CategoryTypeView.Text);
			this.EditCategoryTypeView.SetSelection(selectedIndex);

			var animator = AnimateReveal (this.DisplayLayout, this.EditLayout);

			this.EditLayout.Visibility = ViewStates.Visible;
			this.DisplayLayout.Visibility = ViewStates.Gone;

			animator.Start ();

			this.EditCategoryNameView.RequestFocus ();
			this.inputUtilities.ShowKeyboard (this.EditCategoryNameView);
		}

		protected void OnSave(object sender, EventArgs e)
		{
			this.inputUtilities.HideKeyboard (this.View);

			this.CategoryNameView.Text = this.EditCategoryNameView.Text;
			this.CategoryDescriptionView.Text = this.EditCategoryDescriptionView.Text;
			this.CategoryTypeView.Text = this.EditCategoryTypeView.SelectedItem.ToString();

			var animator = AnimateReveal (this.EditLayout, this.DisplayLayout);

			this.EditLayout.Visibility = ViewStates.Gone;
			this.DisplayLayout.Visibility = ViewStates.Visible;

			animator.Start ();

			Toast.MakeText (this.View.Context, "Category saved: " + this.TagId.Id.ToString(), ToastLength.Short).Show();
		}

		protected void OnCancel(object sender, EventArgs e)
		{
			this.inputUtilities.HideKeyboard (this.View);

			var animator = AnimateReveal (this.EditLayout, this.DisplayLayout);

			this.EditLayout.Visibility = ViewStates.Gone;
			this.DisplayLayout.Visibility = ViewStates.Visible;

			animator.Start ();
		}

		protected void OnDelete(object sender, EventArgs e)
		{
			var builder = new AlertDialog.Builder (this.View.Context);
			builder.SetTitle (Resource.String.confirmDeleteTitle)
				.SetMessage (Resource.String.confirmDeleteMessage)
				.SetPositiveButton (Resource.String.delete, OnDeleteConfirmed)
				.SetNegativeButton (Resource.String.cancel, OnDeleteCancelled);
			builder.Create ().Show ();
		}

		protected void OnDeleteConfirmed(object sender, EventArgs e)
		{
			if (this.ItemDeleted != null) {
				ItemDeletedEventArgs args = new ItemDeletedEventArgs ();
				args.AdapterPosition = this.AdapterPosition;
				args.View = this.View;
				this.ItemDeleted (sender, args);
			}
		}

		protected void OnDeleteCancelled(object sender, EventArgs e)
		{
		}
			
		private Animator AnimateReveal(View currentView, View newView) {
			// use currentView to get the dimensions since displayHolder is not drawn and is 0 x 0
			// get the center for the clipping circle
			int cx = currentView.MeasuredWidth / 2;
			int cy = currentView.MeasuredHeight / 2;

			// get the final radius for the clipping circle
			int finalRadius = Math.Max(currentView.Width, currentView.Height) / 2;
			Animator animator = ViewAnimationUtils.CreateCircularReveal (newView, cx, cy, 0, finalRadius);

			// Set a natural ease-in/ease-out interpolator
			animator.SetInterpolator (new AccelerateDecelerateInterpolator ());

			return animator;
		}
	}
}

