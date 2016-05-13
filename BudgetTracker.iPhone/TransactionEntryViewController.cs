using BudgetTracker;
using BudgetTracker.iPhone.ViewModels;
using Foundation;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace BudgetTracker.iPhone
{
	partial class TransactionEntryViewController : UIViewController
	{
		private UIView categoryPickerView;
		private UIPickerView picker;
		private IList<Category> categories;

		public TransactionEntryViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			await this.CreateCategoryPicker();

			this.CategoryField.EditingDidBegin += CategoryEditingDidBegin;

			this.SaveButton.TouchUpInside += SaveTransaction;
		}

		public override void ViewWillUnload()
		{
			if (this.categories != null)
			{
				this.categories = null;
			}

			if (this.picker != null)
			{
				this.picker.Dispose();
				this.picker = null;
			}

			if (this.SaveButton != null)
			{
				this.SaveButton.TouchUpInside -= SaveTransaction;
			}

			if (this.CategoryField != null)
			{
				this.CategoryField.EditingDidBegin -= CategoryEditingDidBegin;
			}

			if (this.categoryPickerView != null)
			{
				this.categoryPickerView.Dispose();
				this.categoryPickerView = null;
			}

			base.ViewWillUnload();
		}

		protected async void SaveTransaction(object sender, EventArgs e)
		{
			try
			{
				// clear current focus
				if (this.Vendor.IsFirstResponder)
				{
					this.Vendor.ResignFirstResponder();
				}

				if (this.Description.IsFirstResponder)
				{
					this.Description.ResignFirstResponder();
				}

				if (this.Amount.IsFirstResponder)
				{
					this.Amount.ResignFirstResponder();
				}

				var selectedCategoryName = this.CategoryField.Text;
				if (string.IsNullOrWhiteSpace(selectedCategoryName) || this.categories == null)
				{
					Console.WriteLine("Null category(s)");
					return;
				}

				var selectedCategory = this.categories.Where(x => x.Name == selectedCategoryName).FirstOrDefault();
				if (selectedCategory == default(Category))
				{
					Console.WriteLine("Selected non-existent category");
					return;
				}

				var transaction = new Transaction();
				transaction.Amount = Convert.ToDecimal(this.Amount.Text);
				transaction.Vendor = this.Vendor.Text;
				transaction.Description = this.Description.Text;
				transaction.CategoryId = selectedCategory.Id;

				var transactionService = new MockTransactionService();
				await transactionService.InitializeService();
				await transactionService.Insert(transaction);

				UIAlertController alertController = UIAlertController.Create("Transaction Saved", "Your transaction was successfully saved", UIAlertControllerStyle.Alert);
				alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (alertAction) => 
				{
					this.Vendor.Text = "";
					this.Amount.Text = "";
					this.Description.Text = "";
					this.CategoryField.Text = "";
				}));

				this.PresentViewController(alertController, true, null);
			}
			catch(Exception ex)
			{
				System.Console.WriteLine(string.Format("Error saving transaction: {0}: {1}", ex.Message, ex.StackTrace));
			}
		}

		protected void CategoryEditingDidBegin(object sender, EventArgs e)
		{
			this.CategoryField.InputView = categoryPickerView;
		}

		protected void OnCancelCategoryPicker(object sender, EventArgs e)
		{
			if (this.CategoryField.EndEditing(true))
			{
				this.picker.Select(0, 0, false);
			}
		}

		protected void OnDoneCategoryPicker(object sender, EventArgs e)
		{
			if (this.CategoryField.EndEditing(false))
			{
				this.CategoryField.Text = this.categories[(int)this.picker.SelectedRowInComponent(0)].Name;
				this.picker.Select(0, 0, false);
			}
		}

		private async Task CreateCategoryPicker()
		{
			var categoryService = new MockCategoryService();
			var categoryTask = categoryService.RetrieveCategories();

			// create the category picker item
			UIToolbar categoryToolbar = new UIToolbar(RectangleF.Empty);
			categoryToolbar.BarStyle = UIBarStyle.Black;
			categoryToolbar.Translucent = true;
			categoryToolbar.UserInteractionEnabled = true;
			categoryToolbar.SizeToFit();

			UIBarButtonItem btnCancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, OnCancelCategoryPicker);
			UIBarButtonItem btnDone = new UIBarButtonItem(UIBarButtonSystemItem.Done, OnDoneCategoryPicker);
			UIBarButtonItem btnFlexibleSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem[] btnItems = new UIBarButtonItem[] { btnCancel, btnFlexibleSpace, btnDone };
			categoryToolbar.SetItems(btnItems, true);

			picker = new UIPickerView(new RectangleF(0, 44, (float)this.View.Bounds.Width, 216));
			picker.UserInteractionEnabled = true;
			picker.BackgroundColor = UIColor.White;

			this.categoryPickerView = new UIView(new RectangleF(0, 0, (float)this.View.Bounds.Width, 260));
			this.categoryPickerView.AddSubview(categoryToolbar);
			this.categoryPickerView.AddSubview(picker);

			this.categories = await categoryTask;
			picker.Model = new CategoryViewModel(this.categories);
		}
	}
}
