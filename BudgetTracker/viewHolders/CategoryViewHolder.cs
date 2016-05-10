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
	/// <summary>
	/// Category view holder.
	/// </summary>
	public class CategoryViewHolder : RecyclerView.ViewHolder
	{
		private InputUtilities inputUtilities;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BudgetTracker.CategoryViewHolder"/> class.
		/// </summary>
		/// <param name="v">The containing view.</param>
		/// <param name="inputUtilities">An instance of input utilities.</param>
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

		/// <summary>
		/// Occurs when item deleted.
		/// </summary>
		public event EventHandler<ItemDeletedEventArgs> ItemDeleted;

		#region Overrides
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
		#endregion

		/// <summary>
		/// Gets or sets the view.
		/// </summary>
		/// <value>The view.</value>
		public View View {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the category name view.
		/// </summary>
		/// <value>The category name view.</value>
		public TextView CategoryNameView { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the category description view.
		/// </summary>
		/// <value>The category description view.</value>
		public TextView CategoryDescriptionView { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the category type view.
		/// </summary>
		/// <value>The category type view.</value>
		public TextView CategoryTypeView { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the edit category name view.
		/// </summary>
		/// <value>The edit category name view.</value>
		public EditText EditCategoryNameView { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the edit category description view.
		/// </summary>
		/// <value>The edit category description view.</value>
		public EditText EditCategoryDescriptionView { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the edit category type view.
		/// </summary>
		/// <value>The edit category type view.</value>
		public Spinner EditCategoryTypeView { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the edit category button.
		/// </summary>
		/// <value>The edit category button.</value>
		public Button EditCategoryButton { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the cancel category button.
		/// </summary>
		/// <value>The cancel category button.</value>
		public Button CancelCategoryButton { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the save category button.
		/// </summary>
		/// <value>The save category button.</value>
		public Button SaveCategoryButton { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the delete category button.
		/// </summary>
		/// <value>The delete category button.</value>
		public Button DeleteCategoryButton { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the edit layout.
		/// </summary>
		/// <value>The edit layout.</value>
		public View EditLayout { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the display layout.
		/// </summary>
		/// <value>The display layout.</value>
		public View DisplayLayout { 
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the tag identifier.
		/// </summary>
		/// <value>The tag identifier.</value>
		public IdTag TagId
		{
			get {
				return (IdTag)this.View.GetTag (Resource.Id.view_tag_id);
			}
			set {
				this.View.SetTag (Resource.Id.view_tag_id, value);
			}
		}

		/// <summary>
		/// Handles when the spinner is touched.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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

		/// <summary>
		/// Handles when the edit button is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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

		/// <summary>
		/// Handles when the Save button is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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

		/// <summary>
		/// Handles when the cancel button is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCancel(object sender, EventArgs e)
		{
			this.inputUtilities.HideKeyboard (this.View);

			var animator = AnimateReveal (this.EditLayout, this.DisplayLayout);

			this.EditLayout.Visibility = ViewStates.Gone;
			this.DisplayLayout.Visibility = ViewStates.Visible;

			animator.Start ();
		}

		/// <summary>
		/// Handles when the delete button is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnDelete(object sender, EventArgs e)
		{
			var builder = new AlertDialog.Builder (this.View.Context);
			builder.SetTitle (Resource.String.confirmDeleteTitle)
				.SetMessage (Resource.String.confirmDeleteMessage)
				.SetPositiveButton (Resource.String.delete, OnDeleteConfirmed)
				.SetNegativeButton (Resource.String.cancel, OnDeleteCancelled);
			builder.Create ().Show ();
		}

		/// <summary>
		/// Handles when the delete is confirmed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnDeleteConfirmed(object sender, EventArgs e)
		{
			if (this.ItemDeleted != null) {
				ItemDeletedEventArgs args = new ItemDeletedEventArgs ();
				args.AdapterPosition = this.AdapterPosition;
				args.View = this.View;
				this.ItemDeleted (sender, args);
			}
		}

		/// <summary>
		/// Handles when the delete is cancelled.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnDeleteCancelled(object sender, EventArgs e)
		{
		}
			
		/// <summary>
		/// Creates an Animator to handle revealing a new view.
		/// </summary>
		/// <returns>The animator.</returns>
		/// <param name="currentView">Current view.</param>
		/// <param name="newView">New view.</param>
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

