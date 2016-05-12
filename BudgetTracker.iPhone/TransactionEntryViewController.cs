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

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.CategoryPicker.Model = new CategoryViewModel(new List<Category>());
		}
	}
}
