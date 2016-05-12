using BudgetTracker;
using BudgetTracker.iPhone.ViewModels;
using Foundation;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using UIKit;

namespace BudgetTracker.iPhone
{
	partial class TransactionEntryViewController : UIViewController
	{
		public TransactionEntryViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var categoryService = new MockCategoryService();
			var categories = await categoryService.RetrieveCategories();
			this.CategoryPicker.Model = new CategoryViewModel(categories);
			this.SaveButton.TouchUpInside += SaveTransaction;
		}

		public override void ViewWillUnload()
		{
			if (this.CategoryPicker != null)
			{
				this.CategoryPicker.Model = null;
			}

			if (this.SaveButton != null)
			{
				this.SaveButton.TouchUpInside -= SaveTransaction;
			}

			base.ViewWillUnload();
		}

		protected async void SaveTransaction(object sender, EventArgs e)
		{
			var selectedCategory = ((CategoryViewModel)this.CategoryPicker.Model).Items[Convert.ToInt32(this.CategoryPicker.SelectedRowInComponent(0))];
			var transaction = new Transaction();
			transaction.Amount = Convert.ToDecimal(this.Amount.Text);
			transaction.Vendor = this.Vendor.Text;
			transaction.Description = this.Description.Text;
			transaction.CategoryId = selectedCategory.Id;

			var transactionService = new MockTransactionService();
			await transactionService.InitializeService();
			await transactionService.Insert(transaction);

			UIAlertController alert = new UIAlertController();
			alert.Title = "Transaction Saved";
			alert.Message = "This finished";

			this.PresentViewController(alert, true, null);
		}
	}
}
